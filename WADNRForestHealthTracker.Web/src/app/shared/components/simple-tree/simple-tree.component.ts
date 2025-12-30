import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from "@angular/core";

import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { IFeature } from "src/app/shared/generated/model/i-feature";

export interface SimpleTreeNode {
    title: string;
    key: string;
    count?: number;
    children?: SimpleTreeNode[];
    data?: IFeature;
    folder?: boolean;
    // optional hyperlink fields
    href?: string;
    routerLink?: any;
    target?: string;
    // optional mixed title parts: each part can be plain text or a link
    titleParts?: Array<{
        text: string;
        href?: string;
        routerLink?: any;
        target?: string;
    }>;
}

@Component({
    selector: "simple-tree",
    standalone: true,
    imports: [FormsModule, RouterModule],
    templateUrl: "./simple-tree.component.html",
    styleUrls: ["./simple-tree.component.scss"],
})
export class SimpleTreeComponent implements OnInit, OnChanges {
    @Input() nodes: SimpleTreeNode[] | null = [];
    /** Expand nodes down to this depth on init/when nodes change. 0 = none (default) */
    @Input() initialExpandedDepth: number = 0;
    // Legacy boolean input kept for backward compatibility. Prefer `showTopLevelBorder`.
    private _topLevelBorder = false;
    @Input()
    public get topLevelBorder(): boolean {
        return this._topLevelBorder;
    }
    public set topLevelBorder(v: boolean) {
        this._topLevelBorder = !!v;
        // also mirror to newer API for convenience
        this.showTopLevelBorder = !!v;
    }

    // New preferred input name: simple boolean to enable top-level borders
    @Input() showTopLevelBorder: boolean = false;
    // mode: 'badge' = badges-only (full tree, show counts), 'filter' = filter tree to matches
    @Input() mode: "badge" | "filter" = "badge";
    @Output() select = new EventEmitter<IFeature | null>();

    private _searchTerm: string = "";
    public get searchTerm(): string {
        return this._searchTerm;
    }
    public set searchTerm(v: string) {
        this._searchTerm = v || "";
    }

    private expandedKeys = new Set<string>();

    // Expand tree nodes down to this depth (inclusive of parents). Depth 0 = expand none.
    private expandToDepth(depth: number) {
        this.expandedKeys.clear();
        if (!this.nodes || depth <= 0) {
            return;
        }

        const walk = (items: SimpleTreeNode[] | undefined, currentDepth: number) => {
            if (!items || items.length === 0) {
                return;
            }
            for (const n of items) {
                const hasChildren = !!(n.children && n.children.length);
                if (hasChildren && currentDepth < depth) {
                    if (n.key) {
                        this.expandedKeys.add(n.key);
                    }
                    // continue into children
                    walk(n.children, currentDepth + 1);
                }
            }
        };

        walk(this.nodes, 0);
    }

    ngOnInit(): void {
        // apply initial expansion on init
        if (this.initialExpandedDepth && this.initialExpandedDepth > 0) {
            this.expandToDepth(this.initialExpandedDepth);
        }
    }

    ngOnChanges(changes: SimpleChanges): void {
        // if nodes input or initialExpandedDepth changed, reapply expansion
        if (changes["nodes"] || changes["initialExpandedDepth"]) {
            const depth = Number(this.initialExpandedDepth) || 0;
            if (depth > 0) {
                this.expandToDepth(depth);
            }
        }
    }

    isExpanded(key: string): boolean {
        return this.expandedKeys.has(key);
    }

    toggleExpand(key: string) {
        if (!key) {
            return;
        }
        if (this.expandedKeys.has(key)) {
            this.expandedKeys.delete(key);
        } else {
            this.expandedKeys.add(key);
        }
    }

    onNodeClick(node: SimpleTreeNode) {
        if (node.data) {
            this.select.emit(node.data);
            return;
        }
        // toggle folder nodes
        if (node.key) {
            this.toggleExpand(node.key);
        }
    }

    onKeyDown(event: KeyboardEvent, node: SimpleTreeNode) {
        const key = event.key;
        if (key === "Enter" || key === " ") {
            event.preventDefault();
            this.onNodeClick(node);
            return;
        }
        // Simple arrow navigation: up/down move focus between visible items
        if (key === "ArrowDown" || key === "ArrowUp") {
            event.preventDefault();
            this.moveFocus(key === "ArrowDown" ? 1 : -1);
            return;
        }
        if (key === "ArrowRight") {
            // expand
            if (node.children && node.children.length && !this.isExpanded(node.key)) {
                this.toggleExpand(node.key);
            }
            return;
        }
        if (key === "ArrowLeft") {
            // collapse
            if (node.children && node.children.length && this.isExpanded(node.key)) {
                this.toggleExpand(node.key);
            }
            return;
        }
    }

    // Move focus between visible tree headers
    private moveFocus(direction: 1 | -1) {
        const headers = Array.from(document.querySelectorAll(".simple-tree__header")) as HTMLElement[];
        if (!headers || !headers.length) return;
        const active = document.activeElement as HTMLElement | null;
        let idx = headers.findIndex((h) => h === active);
        if (idx === -1) idx = 0;
        const next = Math.max(0, Math.min(headers.length - 1, idx + direction));
        const el = headers[next];
        if (el && typeof el.focus === "function") {
            el.focus();
        }
    }

    // Flatten the tree into a visible list of nodes respecting expanded state
    // Each item includes depth for indentation and a reference to the original node
    public get visibleNodes(): Array<{
        node: SimpleTreeNode;
        depth: number;
        hasChildren: boolean;
        rootId?: string | null;
        isGroupStart?: boolean;
        isGroupEnd?: boolean;
    }> {
        type VisibleItem = {
            node: SimpleTreeNode;
            depth: number;
            hasChildren: boolean;
            rootId?: string | null;
            isGroupStart?: boolean;
            isGroupEnd?: boolean;
        };
        const out: VisibleItem[] = [];
        const nodes = this.displayNodes || [];

        const walk = (items: SimpleTreeNode[] | undefined, depth: number) => {
            if (!items || items.length === 0) {
                return;
            }
            for (const n of items) {
                const hasChildren = !!(n.children && n.children.length);
                out.push({ node: n, depth, hasChildren });
                if (hasChildren && this.isExpanded(n.key)) {
                    walk(n.children, depth + 1);
                }
            }
        };

        walk(nodes, 0);

        // compute rootId for each visible item by searching the original nodes tree
        const computeRootId = (key: string | undefined): string | null => {
            if (!key) {
                return null;
            }
            // search this.nodes for the top-level ancestor whose subtree contains the key
            const roots = this.nodes || [];
            const search = (item: SimpleTreeNode, targetKey: string): boolean => {
                if (item.key === targetKey) {
                    return true;
                }
                if (!item.children) {
                    return false;
                }
                for (const c of item.children) {
                    if (search(c, targetKey)) {
                        return true;
                    }
                }
                return false;
            };

            for (const r of roots) {
                if (search(r, key)) {
                    // extract id portion from root.key e.g. 'area-12' -> '12'
                    const m = (r.key || "").match(/^[^-]+-(.+)$/);
                    return m ? m[1] : null;
                }
            }
            return null;
        };

        for (let i = 0; i < out.length; i++) {
            out[i].rootId = computeRootId(out[i].node?.key);
        }

        // mark group boundaries for contiguous items with the same rootId
        for (let i = 0; i < out.length; i++) {
            const prev = i > 0 ? out[i - 1] : null;
            const next = i < out.length - 1 ? out[i + 1] : null;
            out[i].isGroupStart = !prev || prev.rootId !== out[i].rootId;
            out[i].isGroupEnd = !next || next.rootId !== out[i].rootId;
        }

        return out as any;
    }

    // trackBy function for ngFor
    public trackByNode(index: number, item: { node: SimpleTreeNode }): string {
        return item?.node?.key ?? String(index);
    }

    // helper: return the root-level id for top-level nodes; accepts keys like '<prefix>-<id>' and returns the id portion
    public getRootId(item: { node: SimpleTreeNode; depth: number } | null): string | null {
        if (!item || item.depth !== 0) return null;
        const key = item.node?.key || "";
        const m = key.match(/^[^-]+-(.+)$/);
        return m ? m[1] : null;
    }

    // compute match counts for every node while preserving full tree structure
    // returns a new tree with `count` populated as the number of matching leaves under each node
    get nodesWithMatchCounts(): SimpleTreeNode[] {
        const term = (this.searchTerm || "").trim().toLowerCase();
        const nodes = this.nodes || [];
        if (!term) return nodes;

        const mapCounts = (items: SimpleTreeNode[] | undefined): SimpleTreeNode[] => {
            if (!items || !items.length) return [];
            return items.map((n) => {
                const title = (n.title || "").toLowerCase();
                const isLeaf = !n.children || n.children.length === 0;
                if (isLeaf) {
                    const isMatch = title.indexOf(term) >= 0;
                    return { ...n, count: isMatch ? 1 : 0 };
                }
                const children = mapCounts(n.children);
                const childCount = children.reduce((s, c) => s + (c.count || 0), 0);
                const titleMatches = title.indexOf(term) >= 0;
                return { ...n, children, count: childCount + (titleMatches ? 1 : 0) };
            });
        };

        return mapCounts(nodes);
    }

    // total number of matching leaves across the whole tree (for current search)
    get totalMatches(): number {
        const nodes = this.nodesWithMatchCounts || [];
        return nodes.reduce((s, n) => s + (n.count || 0), 0);
    }

    // filter nodes recursively by search term (case-insensitive)
    // returns pruned tree (only nodes that match or contain matches) and sets counts for matching leaves and parent counts
    get filteredNodes(): SimpleTreeNode[] {
        const term = (this.searchTerm || "").trim().toLowerCase();
        const nodes = this.nodes || [];
        if (!term) return nodes;

        const filter = (items: SimpleTreeNode[] | undefined): SimpleTreeNode[] => {
            if (!items || items.length === 0) return [];
            const out: SimpleTreeNode[] = [];
            for (const n of items) {
                const title = (n.title || "").toLowerCase();
                const isLeaf = !n.children || n.children.length === 0;
                if (isLeaf) {
                    const isMatch = title.indexOf(term) >= 0;
                    if (isMatch) out.push({ ...n, count: 1 });
                } else {
                    const children = filter(n.children);
                    const childCount = children.reduce((s, c) => s + (c.count || 0), 0);
                    const titleMatches = title.indexOf(term) >= 0;
                    if (childCount > 0 || titleMatches) {
                        out.push({ ...n, children, count: childCount });
                    }
                }
            }
            return out;
        };

        return filter(nodes);
    }

    // displayNodes picks the tree shape depending on `mode`
    get displayNodes(): SimpleTreeNode[] {
        return this.mode === "filter" ? this.filteredNodes : this.nodesWithMatchCounts;
    }
}
