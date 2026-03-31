import { Pipe, PipeTransform } from "@angular/core";
import { EditorComponent } from "@tinymce/tinymce-angular";
import TinyMCEHelpers from "../helpers/tiny-mce-helpers";

@Pipe({
    name: "tinyMceConfig",
})
/**
 * Pipe to transform the TinyMCE editor configuration.
 * Used for the TinyMCE editor component to prevent the editor from being re-initialized on every change detection cycle.
 */
export class TinyMceConfigPipe implements PipeTransform {
    transform(editor: EditorComponent, ariaLabel?: string): unknown {
        return TinyMCEHelpers.DefaultInitConfig(editor, null, ariaLabel);
    }
}
