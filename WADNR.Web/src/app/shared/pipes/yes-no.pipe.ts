import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: "yesNo",
})
export class YesNoPipe implements PipeTransform {
    transform(value: boolean | null | undefined, yes: string = "Yes", no: string = "No"): string {
        return value ? yes : no;
    }
}
