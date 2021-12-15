# todo list:
1 Create a table with things that was tested and wasn't. (Mostly for OS depended stuff, like file dialogs, chrome version selection and etc.)

# Make selenium undetectable:
0. __WARNING!!! Some of this tips might be mentioned in [[#Special vars strings etc]]__
1. Remove (or set to false, idk) Navigator.Webdriver Flag
2. Obfuscating JavaScript of Browser Driver EXE. Replace `$cdc_asdjflasutopfhvcZLmcfl_` with string **of the same length**
3. Check out params from here and set up a list of properties that are possible to change: https://amiunique.org/fp. (In this case it would be great to generate it, but if it's impossible or inefficient predefined ones will be good as well)
4. Make selenium less unique: https://amiunique.org/fp.
5. Set screen resolution. (**Before driver launch!**)
6. User agent (**It should be the same as browser + don't generate user agents - use existing one**). There are several approaches:
	* Use existing one and replace it with OS, browser version and etc.
	* Somehow check existing user agent and use it with selenium
7. Realistic Page Flow and avoiding Traps. Selenium should **ALWAYS** travel on site starting from home page + don't go to urls that are not available for user (warn user with this information)
8. Maybe try some proxies (add ability to use proxies, idk)
9. Random delays. But there are lots of details:
	* There should be minimum (fast) and maximum (slow) reaction sometimes
	* Sites check how many time you spend on their website (for example, human can't read article in 5 seconds)
	*  There should be very good balance between different delays. For example, average delays is more usual than min or max
10. Use custom chrome profile. Create new profile and copy all data from old one. (Info about it is in previous doc)
11. If there is an error occurred (like 403, 503 and etc.) would be better to exit (stop bot)
12. Make selenium launch params same as original chrome. Stuff like *enable automation*, *blink features* and other `chrome arguments`
13. In the final script use smooth mouse movements (for example, with curves or find another solution)
14. Add extensions to the browser from original profile (maybe they will be added automatically after copying existing profile, idk)
15. Work with vars, strings that uncover selenium usage. Because this topic is big, it is described below [[#Special vars strings etc]]
16. Modifying selenium responses sounds like a good idea, check out [[#How to use most of vars]]

## Special vars, strings, etc.
### Strings with known meaning (except common `$cdc_`): 
`webdriver`  - located inside `navigator`, set it to `undifiened`
`$chrome_asyncScriptInf` - this thing inside driver binary, i have no idea what is does, need to find out!
### Other (they are might be in selenium responses):
`__webdriverFunc`
`__webdriver_script_function`
`__webdriver_script_func`
`__webdriver_script_fn`
`__$webdriverAsyncExecutor`
`__driver_evaluate`
`__webdriver_evaluate`
`__selenium_evaluate` 
`__fxdriver_evaluate` 
`__driver_unwrapped`
`__webdriver_unwrapped`
`__selenium_unwrapped`
`__fxdriver_unwrapped`
`_Selenium_IDE_Recorder`
`calledSelenium`
`_WEBDRIVER_ELEM_CACHE`
`ChromeDriverw`
`driver-evaluate`
`webdriver-evaluate`
`selenium-evaluate`
`webdriverCommand`
`webdriver-evaluate-response`
`__lastWatirAlert`
`__lastWatirConfirm`
`__lastWatirPrompt`
`_phantom`
`__nightmare`
`_selenium`
`callPhantom`
`callSelenium`
`__webdriver_evaluate`
### How to use most of vars:
There is a possibility that they could be mentioned in a response from webdriver. So program needs to track `requests` and `responses`, this links should help:
https://octopus.com/blog/selenium/15-modifying-http-responses/modifying-http-responses
https://automated-testing.info/t/rabota-s-trafikom-fetch-xhr-pri-pomoshhi-selenium-4-webdriver-primer-vnutri/25503/17
(P.s. However, replacing all of strings is probably a bad idea, so this needs testing and tweaking)
### The best way to hide/change/remove vars:
1. Overriding only base property isn't enough, because there is a `.prototype` property with all vars
2. Overriding getter function is bad idea, because in `js` you can get function code as a string, so this the moment when `3` and `4` come into play
3. We can intercept the call to getter and change the returned value using js `Proxy apply`: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Proxy/Proxy/apply
4. But still there will be weird issue with function name, because of `toString()`. To fix it you can somehow try to find original function name and use it, but i didn't really get it, so here the source answer with more detail:  https://stackoverflow.com/a/69533548/13940541
Maybe checkout this video where `.prototype` is used: https://youtu.be/XS_UMqQalLI