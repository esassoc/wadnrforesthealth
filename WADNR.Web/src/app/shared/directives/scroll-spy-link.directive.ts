import { Directive, ElementRef, HostBinding, HostListener, Input, OnDestroy } from "@angular/core";
import { HostListener as HostListener2 } from "@angular/core";
import { ScrollSpyService } from "../services/scroll-spy.service";
import { Subscription } from "rxjs";

@Directive({ selector: "[appScrollSpyLink]", standalone: true })
export class ScrollSpyLinkDirective implements OnDestroy {
    @Input("appScrollSpyLink") targetId!: string;
    @HostBinding("class.active") isActive = false;
    private sub: Subscription;

    constructor(private el: ElementRef, private spy: ScrollSpyService) {
        this.sub = this.spy.active$.subscribe((id) => {
            this.isActive = id === this.targetId;
        });
    }

    @HostListener("click", ["$event"]) onClick(e: Event) {
        e.preventDefault();
        const el = document.getElementById(this.targetId);
        if (el) {
            el.scrollIntoView({ behavior: "smooth", block: "start" });
            // ensure active is set immediately
            this.spy.setActive(this.targetId);
        }
    }

    @HostListener2("keydown", ["$event"]) onKeydown(e: KeyboardEvent) {
        if (e.key === "Enter" || e.key === " ") {
            e.preventDefault();
            this.onClick(e as unknown as Event);
        }
    }

    ngOnDestroy() {
        this.sub.unsubscribe();
    }
}
