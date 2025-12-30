import { Directive, ElementRef, Input, OnDestroy, AfterViewInit, NgZone, Renderer2 } from "@angular/core";

@Directive({ selector: "[appScrollSpySticky]", standalone: true })
export class ScrollSpyStickyDirective implements AfterViewInit, OnDestroy {
    /**
     * Selector or id of the content area the spy should avoid overlapping.
     * Default: `.outcome__content` which matches the outcome detail layout.
     */
    @Input("appScrollSpySticky") contentSelector: string = ".outcome__content";

    /** Optional selector for header whose height should be considered when pinning */
    @Input() topOffset: number = 0;

    private scrollSpyEl?: HTMLElement;
    private imgListeners: Array<() => void> = [];
    private destroyed = false;
    private mutationObserver?: MutationObserver;
    private resizeObserver?: ResizeObserver;
    private rafId: number | null = null;

    private safeSelector(selector: string | undefined, fallback: string) {
        if (selector === undefined || selector === null) return fallback;
        const s = selector.toString().trim();
        return s.length > 0 ? s : fallback;
    }

    constructor(private host: ElementRef<HTMLElement>, private zone: NgZone, private renderer: Renderer2) {}

    ngAfterViewInit(): void {
        // host is the spy container (element with this directive)
        this.scrollSpyEl = this.host.nativeElement;

        // initial calculation after layout stabilizes (give images and fonts a bit more time)
        setTimeout(() => this.calculateAndUpdate(), 150);

        // attach listeners
        this.zone.runOutsideAngular(() => {
            window.addEventListener("scroll", this.onScroll);
            window.addEventListener("resize", this.onResize);
        });

        // attach image load listeners inside the content area
        this.attachImageLoadListeners();

        // observe content changes so we can recalc when async content loads or template changes
        const contentSel = this.safeSelector(this.contentSelector, ".outcome__content");
        const contentEl = document.querySelector(contentSel) as HTMLElement | null;
        if (contentEl) {
            try {
                this.mutationObserver = new MutationObserver(() => this.debouncedCalc());
                this.mutationObserver.observe(contentEl, { childList: true, subtree: true, attributes: true });
            } catch (e) {
                // MutationObserver not supported or blocked; ignore
            }

            try {
                // ResizeObserver helps when injected components resize themselves
                this.resizeObserver = new ResizeObserver(() => this.debouncedCalc());
                this.resizeObserver.observe(contentEl);
            } catch (e) {
                // ResizeObserver not available in some older browsers/environments
            }
        }
    }

    ngOnDestroy(): void {
        this.destroyed = true;
        this.removeImageLoadListeners();
        window.removeEventListener("scroll", this.onScroll);
        window.removeEventListener("resize", this.onResize);
        if (this.mutationObserver) {
            try {
                this.mutationObserver.disconnect();
            } catch (e) {
                // ignore
            }
            this.mutationObserver = undefined;
        }
        if (this.resizeObserver) {
            try {
                this.resizeObserver.disconnect();
            } catch (e) {
                // ignore
            }
            this.resizeObserver = undefined;
        }
        if (this.rafId != null) {
            cancelAnimationFrame(this.rafId);
            this.rafId = null;
        }
    }

    /** Public API: force recalculation (useful when content changes) */
    public recalculate(): void {
        if (this.destroyed) return;
        this.calculateAndUpdate();
        this.attachImageLoadListeners();
    }

    private onScroll = () => this.updateSticky();
    private onResize = () => this.debouncedCalc();

    private debouncedCalc() {
        if (this.destroyed) return;
        if (this.rafId != null) {
            cancelAnimationFrame(this.rafId);
            this.rafId = null;
        }
        this.rafId = requestAnimationFrame(() => {
            this.rafId = null;
            this.calculateAndUpdate();
        });
    }

    private calculateAndUpdate() {
        if (!this.scrollSpyEl) return;

        // compute geometry
        const rect = this.scrollSpyEl.getBoundingClientRect();
        const left = Math.round(rect.left);
        const width = Math.round(rect.width);
        const height = Math.round(this.scrollSpyEl.offsetHeight);
        const originalTop = rect.top + (window.scrollY || window.pageYOffset);

        // store as CSS vars used by .scrollspy-fixed
        this.setVars({ "--spy-top": `${this.topOffset}px`, "--spy-left": `${left}px`, "--spy-width": `${width}px` });

        // stash dataset for updateSticky to use
        this.scrollSpyEl.dataset["spyOriginalTop"] = originalTop.toString();
        this.scrollSpyEl.dataset["spyHeight"] = height.toString();
        this.scrollSpyEl.dataset["spyLeft"] = left.toString();
        this.scrollSpyEl.dataset["spyWidth"] = width.toString();

        // run initial update
        this.updateSticky();
    }

    private updateSticky() {
        if (!this.scrollSpyEl) return;
        const contentSel = this.safeSelector(this.contentSelector, ".outcome__content");
        const contentEl = document.querySelector(contentSel) as HTMLElement | null;
        const originalTop = Number(this.scrollSpyEl.dataset["spyOriginalTop"] || 0);
        const spyHeight = Number(this.scrollSpyEl.dataset["spyHeight"] || this.scrollSpyEl.offsetHeight);
        const left = Number(this.scrollSpyEl.dataset["spyLeft"] || 0);
        const width = Number(this.scrollSpyEl.dataset["spyWidth"] || this.scrollSpyEl.offsetWidth);

        if (!contentEl || !originalTop) return;

        const scrollY = window.scrollY || window.pageYOffset;
        const topOffset = Number((getComputedStyle(this.scrollSpyEl).getPropertyValue("--spy-top") || `${this.topOffset}px`).replace("px", "")) || 20;

        const contentTop = contentEl.getBoundingClientRect().top + scrollY;
        const contentBottom = contentTop + contentEl.offsetHeight;

        const wouldBeTop = scrollY + topOffset;
        const wouldBeBottom = wouldBeTop + spyHeight;

        let shouldFix = wouldBeTop >= originalTop && wouldBeBottom < contentBottom;
        let finalTop = topOffset;
        if (wouldBeTop >= originalTop && wouldBeBottom >= contentBottom) {
            shouldFix = true;
            const maxTop = contentBottom - spyHeight - scrollY;
            finalTop = Math.max(8, Math.round(maxTop));
        }

        const isFixed = this.scrollSpyEl.classList.contains("scrollspy-fixed");
        if (shouldFix && !isFixed) {
            // apply fixed class and set vars to current left/width/finalTop
            this.renderer.addClass(this.scrollSpyEl, "scrollspy-fixed");
            this.setVars({ "--spy-top": `${finalTop}px`, "--spy-left": `${left}px`, "--spy-width": `${width}px` });
        } else if (!shouldFix && isFixed) {
            this.renderer.removeClass(this.scrollSpyEl, "scrollspy-fixed");
            // clear inline vars so CSS sticky state returns
            this.clearVars(["--spy-top", "--spy-left", "--spy-width"]);
        } else if (shouldFix && isFixed) {
            // still fixed — ensure left/width updated (responsive)
            this.setVars({ "--spy-left": `${left}px`, "--spy-width": `${width}px`, "--spy-top": `${finalTop}px` });
        }
    }

    private attachImageLoadListeners() {
        this.removeImageLoadListeners();
        const contentSel = this.safeSelector(this.contentSelector, ".outcome__content");
        const contentEl = document.querySelector(contentSel) as HTMLElement | null;
        if (!contentEl) return;
        const imgs = Array.from(contentEl.querySelectorAll("img")) as HTMLImageElement[];
        imgs.forEach((img) => {
            if (img.complete) return;
            const handler = () => {
                this.debouncedCalc();
            };
            img.addEventListener("load", handler);
            img.addEventListener("error", handler);
            this.imgListeners.push(() => {
                img.removeEventListener("load", handler);
                img.removeEventListener("error", handler);
            });
        });
    }

    private removeImageLoadListeners() {
        this.imgListeners.forEach((fn) => fn());
        this.imgListeners = [];
    }

    private setVars(vars: { [key: string]: string }) {
        if (!this.scrollSpyEl) return;
        Object.keys(vars).forEach((k) => this.scrollSpyEl!.style.setProperty(k, vars[k]));
    }

    private clearVars(keys: string[]) {
        if (!this.scrollSpyEl) return;
        keys.forEach((k) => this.scrollSpyEl!.style.removeProperty(k));
    }
}
