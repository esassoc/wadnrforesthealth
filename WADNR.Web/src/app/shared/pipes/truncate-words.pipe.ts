import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: "truncateWords",
    standalone: true,
})
export class TruncateWordsPipe implements PipeTransform {
    transform(value: string | null | undefined, maxWords: number): string {
        const text = (value ?? "").trim();
        if (!text) {
            return "";
        }

        const words = text.split(/\s+/).filter(Boolean);
        if (!maxWords || maxWords <= 0 || words.length <= maxWords) {
            return text;
        }

        return `${words.slice(0, maxWords).join(" ")}…`;
    }
}
