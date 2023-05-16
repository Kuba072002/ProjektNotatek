// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function updateNoteContent() {
    let wrightArea = document.getElementById("text-input");
    document.getElementById('NoteContent').value = wrightArea.innerHTML;
}

function addLink() {
    var a = document.createElement('a');
    var url = document.getElementById("link-url-input").value;
    a.href = url;

    var linktext = document.getElementById("linktext-input").value;
    if (linktext) {
        a.textContent = linktext;
    }
    else {
        a.textContent = url;
    }
    var textInputDiv = document.getElementById("text-input");
    textInputDiv.appendChild(a);
}

function addPhoto() {
    var url = document.getElementById("url-input").value;
    var img = document.createElement("img");
    img.src = url;

    var alignmentOption = document.getElementById("alignment-input").value;
    var captionText = document.getElementById("caption-input").value;

    if (alignmentOption === "2" || captionText) {
        img.style.display = "block";
        img.style.margin = "0 auto";
    } else if (alignmentOption === "3") {
        img.style.float = "right";
    } else if (alignmentOption === "1") {
        img.style.float = "left";
    }

    var sizeOption = document.getElementById("size-input").value;
    if (sizeOption === "1") {
        img.style.maxWidth = "100%";
    } else if (sizeOption === "2") {
        img.style.width = "50%";
    } else if (sizeOption === "3") {
        img.style.width = "75%";
    }

    var figure = document.createElement("figure");
    figure.className = "text-center";
    figure.appendChild(img);
   
    if (captionText) {
        var caption = document.createElement("figcaption");
        caption.style.textAlign = "center";
        caption.style.fontStyle = "italic";
        caption.innerText = captionText;
        figure.appendChild(caption);
    }

    var textInputDiv = document.getElementById("text-input");
    textInputDiv.appendChild(figure);
}