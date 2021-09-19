function isUrlValid(string) {
    let url;
    try {
        url = new URL(string);
    } catch (_) {
        return false;
    }
    return url.protocol === "http:" || url.protocol === "https:";
}
function displayErrorMessage() {
    document.getElementById("wrong_id_error").setAttribute("class", "error-message");
}
function handleParserStart() {
    if (isUrlValid(search_node.value)) {
        document.getElementById("wrong_id_error").setAttribute("class", "error-message hidden");
        // Redirecting user to inputted url, so:
        // 1) New page will be opened
        // 2) C# code with selenium will be notified of url change
        window.location.replace(search_node.value)
    }
    else { displayErrorMessage(); }
}

const getElement = async element_id => {
    while (document.getElementById(element_id) === null) {
        await new Promise( resolve => requestAnimationFrame(resolve) )
    }
    return document.getElementById(element_id);
};
let search_node = null;
let button_start = null;
// Search (input tag)
getElement("search").then((element) => {
    search_node = element;
    search_node.addEventListener("keyup", function(event) {
        if (event.key === "Enter") {
            handleParserStart();
        }});
});
// Button
getElement("parser-start").then((element) => {
    button_start = element;
    button_start.addEventListener("click", handleParserStart);
});
