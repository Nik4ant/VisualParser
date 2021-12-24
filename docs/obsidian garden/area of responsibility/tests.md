# Tests info about OS depended stuff
Project contains some code for OS depended stuff that should be tested. This section here is used to simplify this testing process (because amount of OS specific code might increase)

### Testing Environments:
* Windows 10 Pro x64 (2009 build 19042)
* Ubuntu 20.04 LTS

| Feature to test       | Windows | Linux   | MacOS   |
| --------------------- | ------- | ------- | ------- |
| Get chrome version    | &check; | &check; | &cross; |
| Single file dialog    | &check; | &check; | &cross; |
| Chrome installation   | &check; | &check; | &cross; |
| Driver installation   | &check; | &check; | &cross; |
| Working chrome driver | &check; | &check; | &cross; | 

# Founded issues after testing:
0) During `dotnet publish` with `IL trimming` some json stuff could possibly go wrong
1) ~~Parsed version contains `/r/n` on the end (need to remove them)~~
2) ~~Driver folder creates inside `root` directory (like `C:/` on Windows)~~
3) ~~File dialog not working (Windows)~~
4) Cross platform chrome installation (windows already done):
	1) ~~Implementation for Linux (with `.deb`)~~
	2) Implementation for mac (with `.dmg`)
5) ~~Weird `? ? [y/n]` instead of `? [y/n]` (because with `ColorConsole` it was trying to parse this: `[y/n]`)~~
7) ~~Bad  `bash` commands execution (bug because of using `-c *command*` instead of `-c "*command*"`). Because of that issue `zenity`, chrome installation and version parsing  wasn't working~~
8) ~~Also chrome version parsing wasn't working because of using `echo` command with  \' symbol instead of \` (first one is for text, second one is for printing result of a command)~~
10) ~~Program doesn't wait for end of chrome installation (because bash command had syntax error)~~
11) ~~`chmod u+x` for chrome isn't working because of wrong path (i have no idea...It works in debug, but doesn't work in release. Still have no idea, but i fixed it with `"` brackets)~~