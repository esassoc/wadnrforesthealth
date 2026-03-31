import { Directive, ElementRef, Input, OnChanges, Renderer2, SimpleChanges } from "@angular/core";
import { DomSanitizer } from "@angular/platform-browser";
import { SecurityContext } from "@angular/core";

@Directive({
    selector: "[oneTimeSrc]",
    standalone: true,
})
export class OneTimeSrcDirective implements OnChanges {
    @Input() oneTimeSrc?: string | any | null;

    constructor(private el: ElementRef<HTMLElement>, private renderer: Renderer2, private sanitizer: DomSanitizer) {}

    ngOnChanges(changes: SimpleChanges): void {
        try {
            const already = this.el.nativeElement.getAttribute("data-once-src-set");
            if (already) return;
            const val = this.oneTimeSrc;
            if (!val) return;
            let url: string | null = null;
            if (typeof val === "string") {
                url = val;
            } else {
                url = this.sanitizer.sanitize(SecurityContext.RESOURCE_URL, val) || null;
            }
            if (url) {
                this.renderer.setAttribute(this.el.nativeElement, "src", url);
                this.renderer.setAttribute(this.el.nativeElement, "data-once-src-set", "true");
            }
        } catch (e) {
            // swallow - not critical
        }
    }
}
