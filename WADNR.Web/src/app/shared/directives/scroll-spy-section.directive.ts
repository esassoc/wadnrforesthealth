import { Directive, ElementRef, Input, OnDestroy, OnInit } from "@angular/core";
import { ScrollSpyService } from "../services/scroll-spy.service";

@Directive({ selector: "[appScrollSpySection]", standalone: true })
export class ScrollSpySectionDirective implements OnInit, OnDestroy {
    @Input("appScrollSpySection") id!: string;
    private observer?: IntersectionObserver;

    constructor(private el: ElementRef<HTMLElement>, private spy: ScrollSpyService) {}

    ngOnInit(): void {
        // ensure the host element has the expected id so links can find it
        if (this.id) {
            try {
                this.el.nativeElement.setAttribute("id", this.id);
            } catch (e) {
                // ignore if unable to set attribute
            }
        }
        this.observer = new IntersectionObserver(
            (entries) => {
                entries.forEach((entry) => {
                    if (entry.isIntersecting) {
                        this.spy.setActive(this.id);
                    }
                });
            },
            { root: null, rootMargin: "0px 0px -60% 0px", threshold: [0.1, 0.5, 1] }
        );
        this.observer.observe(this.el.nativeElement);
    }

    ngOnDestroy(): void {
        this.observer?.disconnect();
    }
}
