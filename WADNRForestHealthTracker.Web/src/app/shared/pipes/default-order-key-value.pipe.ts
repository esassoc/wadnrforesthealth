import { Pipe, PipeTransform } from "@angular/core";
import { KeyValuePipe } from "@angular/common";

const keepOrder = (a, b) => a;

// This pipe uses the angular keyvalue pipe. but doesn't change order.
@Pipe({
    name: "defaultOrderKeyvalue",
    standalone: true,
})
export class DefaultOrderKeyvaluePipe extends KeyValuePipe implements PipeTransform {
    transform(value: any, ...args: any[]): any {
        return super.transform(value, keepOrder);
    }
}
