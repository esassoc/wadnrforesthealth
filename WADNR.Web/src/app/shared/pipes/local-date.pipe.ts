import { Pipe, PipeTransform } from "@angular/core";
import { DatePipe } from "@angular/common";

@Pipe({ name: "localDate", standalone: true })
export class LocalDatePipe implements PipeTransform {
    private datePipe = new DatePipe("en-US");

    transform(value: string | null | undefined, format: string = "mediumDate"): string {
        if (!value) return "\u2014";
        // Append T00:00:00 to date-only strings to force local-time interpretation
        // (without this, "2025-06-15" is parsed as UTC midnight → shows previous day in Pacific)
        const safe = value.includes("T") ? value : `${value}T00:00:00`;
        return this.datePipe.transform(safe, format) ?? "\u2014";
    }
}
