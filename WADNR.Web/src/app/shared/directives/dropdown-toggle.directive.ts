import { Directive, Input, ElementRef, HostListener, Renderer2, OnDestroy } from "@angular/core";
import { NavigationEnd, Router } from "@angular/router";
import { Subscription } from "rxjs";

@Directive({
    selector: "[dropdownToggle]",
    exportAs: "dropdownToggleName",
})
export class DropdownToggleDirective implements OnDestroy {
    private routerNavigationEndSubscription = Subscription.EMPTY;
    private classString: string = "active";
    private scrollListener: (() => void) | null = null;
    @Input() dropdownToggle: any;
    @Input() dropdownToggleFixed: boolean = false;

    showMenu: boolean = false;

    @HostListener("click", ["$event"]) onClick(event) {
        this.showMenu = !this.showMenu;
        this.toggleMenu();
    }

    constructor(private el: ElementRef, private renderer: Renderer2, private router: Router) {
        this.renderer.listen("window", "click", (e: Event) => {
            if (e.target !== this.el.nativeElement && !this.el.nativeElement.contains(e.target) && e.target !== this.dropdownToggle && !this.dropdownToggle.contains(e.target)) {
                this.showMenu = false;
                this.toggleMenu();
            }
        });

        this.routerNavigationEndSubscription = router.events.subscribe((e) => {
            if (e instanceof NavigationEnd) {
                this.showMenu = false;
                this.toggleMenu();
            }
        });
    }

    ngOnDestroy(): void {
        this.routerNavigationEndSubscription.unsubscribe();
        this.removeScrollListener();
    }

    toggleMenu(show: boolean = null) {
        if (show != null) {
            this.showMenu = show;
        }
        if (this.showMenu) {
            this.renderer.addClass(this.el.nativeElement, this.classString);
            this.renderer.addClass(this.dropdownToggle, this.classString);
            if (this.dropdownToggleFixed) {
                this.positionFixedMenu();
                this.addScrollListener();
            }
        } else {
            this.renderer.removeClass(this.el.nativeElement, this.classString);
            if (this.dropdownToggle) {
                this.renderer.removeClass(this.dropdownToggle, this.classString);
            }
            this.el.nativeElement.blur();
            this.removeScrollListener();
        }
    }

    private positionFixedMenu(): void {
        const triggerRect = this.el.nativeElement.getBoundingClientRect();
        const top = triggerRect.bottom + 4;
        const left = triggerRect.left;

        this.renderer.setStyle(this.dropdownToggle, "top", `${top}px`);
        this.renderer.setStyle(this.dropdownToggle, "left", `${left}px`);
    }

    private addScrollListener(): void {
        this.removeScrollListener();
        const handler = () => {
            this.showMenu = false;
            this.toggleMenu();
        };
        window.addEventListener("scroll", handler, true);
        this.scrollListener = () => window.removeEventListener("scroll", handler, true);
    }

    private removeScrollListener(): void {
        if (this.scrollListener) {
            this.scrollListener();
            this.scrollListener = null;
        }
    }
}
