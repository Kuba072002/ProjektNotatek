// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


let optionsButtons = document.querySelectorAll(".option-button");
let wrightArea = document.getElementById("text-input");
let formatButtons = document.querySelectorAll(".format");
let advancedOptionButton = document.querySelectorAll(".adv-option-button");

const initializer = () => {
    higlighter(formatButtons, false);
}

const higlighter = (className, needsRemoval) => {
    className.forEach((button) => {
        button.addEventListener("click", () => {
            if (!needsRemoval) {
                button.classList.toggle("active");
            }
        });
    });
}

const higlighterRemover = (className) => {
    className.forEach((button) => {
        button.classList.remove("active");
    });
}

const modifyText = (command, defaultUi, value) => {
    document.execCommand(command, defaultUi, value);
}

optionsButtons.forEach((button) => {
    button.addEventListener("click", () => {
        modifyText(button.id, false, null);
    })
});

advancedOptionButton.forEach((button) => {
    button.addEventListener("change", () => {
        modifyText(button.id, false, button.value);
    });
});

function updateNoteContent() {
    document.getElementById('NoteContent').value = wrightArea.innerHTML;
}

window.onload = initializer();

function link() {
    if (window.getSelection().toString()) {
        var a = document.createElement('a');
        a.href = window.getSelection().toString();
        window.getSelection().getRangeAt(0).surroundContents(a);
    }
}

function photo() {
    if (window.getSelection().toString()) {
        var img = document.createElement('img');
        img.src = window.getSelection().toString();
        window.getSelection().getRangeAt(0).surroundContents(img);
    }
}