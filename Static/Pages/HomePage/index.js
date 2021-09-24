const allowed_protocols_list = ["http:", "https:", "file:"]

function isUrlValid(string) {
    let url;
    try {
        url = new URL(string);
    } catch (_) {
        return false;
    }
    return allowed_protocols_list.includes(url.protocol);
}
function displayErrorMessage(text) {
    error_message_node.style.visibility = "visible";
    error_message_node.style.opacity = "1";
    error_message_node.innerText = text;
}
function handleParserStart() {
    // Warning user if he want to parse current page
    if (search_node.value === document.URL) {
        displayErrorMessage("ERROR! You can't parse current page")
    }
    else if (isUrlValid(search_node.value)) {
        error_message_node.style.visibility = "hidden";
        error_message_node.style.opacity = "0";
        // Redirecting user to inputted url, so:
        // 1) New page will be opened
        // 2) C# code with selenium will be notified of url change
        window.location.replace(search_node.value)
    }
    else { displayErrorMessage("ERROR! Invalid url!"); }
}

const getElement = async element_id => {
    while (document.getElementById(element_id) === null) {
        await new Promise( resolve => requestAnimationFrame(resolve) )
    }
    return document.getElementById(element_id);
};
let error_message_node = null;
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
// Error message
getElement("wrong_id_error").then((element) => {
    error_message_node = element;
});