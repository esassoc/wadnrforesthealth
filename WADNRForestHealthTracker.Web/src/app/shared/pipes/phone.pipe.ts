import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: "phone",
})
export class PhonePipe implements PipeTransform {
    transform(value: string): string {
        if (!value && value !== "") return value;

        // Normalize by removing all non-digit characters
        const digits = (value ?? "").toString().replace(/\D/g, "");

        // Handle leading '1' country code for US numbers
        let norm = digits;
        if (norm.length === 11 && norm.startsWith("1")) {
            norm = norm.substring(1);
        }

        // Format 10-digit numbers as (AAA) BBB-CCCC
        if (norm.length === 10) {
            const area = norm.substring(0, 3);
            const mid = norm.substring(3, 6);
            const last = norm.substring(6, 10);
            return `(${area}) ${mid}-${last}`;
        }

        // Format 7-digit numbers as BBB-CCCC
        if (norm.length === 7) {
            const mid = norm.substring(0, 3);
            const last = norm.substring(3, 7);
            return `${mid}-${last}`;
        }

        // If not a 7/10 digit number, return original input
        return value;
    }

    gridFilterTextFormatter(from: string) {
        if (!from && from !== "") return from;
        return (from ?? "").toString().replace(/\D/g, "");
    }
}
