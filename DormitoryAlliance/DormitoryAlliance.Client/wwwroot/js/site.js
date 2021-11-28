﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    let PlaceHolderHereElement = $('#PlaceHolderHere');
    $('button[data-toggle="ajax-modal"]').click(function (event){
        let url = $(this).data('url');
        $.get(url).done(function (data) {
            PlaceHolderHereElement.html(data);
            PlaceHolderHereElement.find('.modal').modal('show');
        })
    })
})