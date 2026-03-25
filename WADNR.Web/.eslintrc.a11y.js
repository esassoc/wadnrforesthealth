/**
 * Minimal ESLint config for HTML template accessibility auditing.
 * Used by /audit-a11y skill to avoid TypeScript parser conflicts.
 */
module.exports = {
    "root": true,
    "overrides": [
        {
            "files": ["*.html"],
            "parser": "@angular-eslint/template-parser",
            "plugins": ["@angular-eslint/template"],
            "rules": {
                "@angular-eslint/template/alt-text": "warn",
                "@angular-eslint/template/click-events-have-key-events": "warn",
                "@angular-eslint/template/interactive-supports-focus": "warn",
                "@angular-eslint/template/label-has-associated-control": "warn",
                "@angular-eslint/template/no-positive-tabindex": "error",
                "@angular-eslint/template/role-has-required-aria": "warn",
                "@angular-eslint/template/valid-aria": "warn",
                "@angular-eslint/template/elements-content": "warn",
                "@angular-eslint/template/no-autofocus": "warn",
                "@angular-eslint/template/table-scope": "warn"
            }
        }
    ]
};
