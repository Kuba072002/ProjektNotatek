// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function updateNoteContent() {
    document.getElementById('NoteContent').value = wrightArea.innerHTML;
}

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