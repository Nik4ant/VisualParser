function isUrlValid(string) {
    let url;
    try {
        url = new URL(string);
    } catch (_) {
        return false;
    }
    return url.protocol === "http:" || url.protocol === "https:";
}
function displayErrorMessage(text) {
    alert(text);
    // <div class="bar error">Error message</div>
}
function handleParserStart() {
    if (isUrlValid(search_node.value)) {
        // Setting the attribute value so that  C# code with selenium
        // can see parser start event
        button_start.setAttribute("url_to_parse", search_node.value);
    }
    else { displayErrorMessage(invalidUrlErrorText); }
}

const getElement = async element_id => {
    while (document.getElementById(element_id) === null) {
        await new Promise( resolve => requestAnimationFrame(resolve) )
    }
    return document.getElementById(element_id);
};
let search_node = null;
let button_start = null;
const invalidUrlErrorText = "Error! Invalid url";
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
