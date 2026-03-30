import { Directive, ElementRef, Input, OnDestroy, AfterViewInit } from "@angular/core";

@Directive({ selector: "[countUp]", standalone: true })
export class CountUpDirective implements AfterViewInit, OnDestroy {
    @Input("countUp") endValue = 0;
    @Input() duration = 1750;
    @Input() startWhenVisible = true;

    private rafId: number | null = null;
    private io?: IntersectionObserver;

    constructor(private el: ElementRef<HTMLElement>) {}

    ngAfterViewInit(): void {
        if (this.startWhenVisible) {
            this.io = new IntersectionObserver(
                ([entry]) => {
                    if (entry.isIntersecting) {
                        this.io?.disconnect();
                        this.start();
                    }
                },
                { threshold: 0.25 }
            );
            this.io.observe(this.el.nativeElement);
        } else {
            this.start();
        }
    }

    ngOnDestroy(): void {
        if (this.rafId != null) cancelAnimationFrame(this.rafId);
        this.io?.disconnect();
    }

    private start() {
        const startTs = performance.now();
        const step = (now: number) => {
            const t = Math.min(1, (now - startTs) / this.duration);
            const eased = 1 - Math.pow(1 - t, 2);
            const current = Math.round(this.endValue * eased);
            this.el.nativeElement.textContent = current.toLocaleString();
            if (t < 1) this.rafId = requestAnimationFrame(step);
        };
        this.rafId = requestAnimationFrame(step);
    }
}
