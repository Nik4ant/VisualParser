function calculateBrowserZoom() {
    const searchRange = [0.2, 3];
    let [low, high] = searchRange;
    let lastStep = Infinity;
    const precision = 0.001;
    while (lastStep > precision) {
        lastStep = (high - low);
        const p = lastStep / 2 + low;
        const inlo = window.matchMedia(`(min-resolution: ${low-0.01}dppx) and (max-resolution: ${p}dppx)`).matches;
        if (inlo) {
            high = p;
        } else {
            low = p
        }
    }
    return high.toFixed(2);
}


const localHostUrl = "http://localhost:8888";
// Posting info to HttpListener in C# code
fetch(
    new Request(localHostUrl, {
        method: 'POST',
        mode: 'no-cors',
        credentials: 'include',
        cache: 'no-cache',
        body: JSON.stringify({
            'userAgent': navigator.userAgent,
            'zoom': calculateBrowserZoom().toString(),
            'resolution': `${window.screen.width}, ${window.screen.height}`,
        }),
        headers: new Headers({ 'Content-Type': 'application/json' })
    })
)
    .catch(error => {
        console.log(error)
        // bug: C# code freezes (seams like infinite wait http request)
        window.close();
    });
console.log("Request was sent")  // just for debug
window.close();