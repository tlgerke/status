This scripts folder contains helper PowerShell scripts used during development.

Files:
- enable-serilog.ps1 — adds the SERILOG conditional symbol to a project and can apply replacements.
- apply-serilog-replacements.ps1 — replaces `Helpers.Logging` usages with `Serilog.Log` calls (creates .bak backups).

Note: These scripts modify source files. Run locally and review .bak files.
