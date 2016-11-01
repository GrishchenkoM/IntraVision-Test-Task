
var currentBtns = []; // display list certain keys this modal window

var content = {}; // window content storage

// content reading
function readContent() {
    var id = $('#coinId').val();
    if (id != '') id = parseInt($('#coinId').val());
    else id = -1;

    content.Id = id;
    content.Name = $('#coinName').val();
    content.IsAvailable = $('#isAvailableSelect :selected').val();
    content.Number = $('#coinNumber').val();
}

// coin creation - POST
function CreateCoin() {

    readContent();

    var viewModel = {
        Id: content.Id,
        Name: content.Name,
        IsAvailable: content.IsAvailable,
        Number: content.Number
    }

    $.ajax({
        url: '/api/coin/',
        type: 'POST',
        data: viewModel,
        success: function(model) {
            window.UpdateIndexContent(model, '#modCoinDialog');
        },
        error: function(data) {
            $('#coinResult').css("display", "block");
            $('#coinResult').append(data.responseText);
        }
    });
}

// modification of coin - PUT
function UpdateCoin() {

    readContent();

    var viewModel = {
        Id: content.Id,
        Name: content.Name,
        IsAvailable: content.IsAvailable,
        Number: content.Number
    }

    $.ajax({
        url: '/api/coin/',
        type: 'PUT',
        data: viewModel,
        success: function(model) {
            window.UpdateIndexContent(model, '#modCoinDialog');
        },
        error: function(data) {
            $('#coinResult').css("display", "block");
            $('#coinResult').append(data.responseText);
        }
    });
}

// deleting of coin - DELETE
function DeleteCoin() {

    readContent();

    $.ajax({
        url: '/api/coin/' + content.Id,
        type: 'DELETE',
        success: function(model) {
            window.UpdateIndexContent(model, '#modCoinDialog');
        },
        error: function(data) {
            $('#coinResult').css("display", "block");
            $('#coinResult').append(data.responseText);
        }
    });
}

// button on modal view. Shows when all form elements are filled
function displayButton() {
    var name = $('#coinName').val().replace(new RegExp('\n', 'g'), '').replace(new RegExp(' ', 'g'), '');
    var number = $('#coinNumber').val().replace(new RegExp('\n', 'g'), '').replace(new RegExp(' ', 'g'), '');
    //var authorNameInput = $('#authorNameInput').val().replace(new RegExp('\n', 'g'), '').replace(new RegExp(' ', 'g'), '');
    if (name !== '' && number !== '') {
        for (var i = 0; i < currentBtns.length; ++i) {
            $('#coinResult').text('');
            $('#coinResult').css("display", "none");
            $(currentBtns[i]).css("display", "block");
        }
    } else {
        for (var i = 0; i < currentBtns.length; ++i) {
            $(currentBtns[i]).css("display", "none");
        }
    }
}

// request for info about the authors - GET -------------------- !!
function getAuthors() {
    $.ajax({
        url: '/api/coin',
        type: 'GET',
        dataType: 'json',
        success: function(data) {
            var sel = document.getElementById('authorNameSelect');
            var opt = null;

            for (var i = -1; i < data.length; ++i) {
                opt = document.createElement('option');
                if (i == -1) {
                    opt.value = '';
                    opt.id = i;
                    opt.innerHTML = '';
                } else {
                    opt.value = data[i].Name;
                    opt.id = data[i].Id;
                    opt.innerHTML = data[i].Name;
                }
                sel.appendChild(opt);
            }
        },
        error: function(data) {
            $('#bookResult').css("display", "block");
            $('#bookResult').append(data.responseText);
        }
    });
}

// request for coin list - GET
function getCoin(model) {
    echo(model);
    $.ajax({
        url: '/api/coin/' + model,
        type: 'GET',
        //data: { index: model },
        dataType: 'json',
        success: function(data) {
            echo(data);
            $('#coinId').val(data.Id);
            $('#coinName').val(data.Name);
            $('#coinNumber').val(data.Number);
            $("#isAvailableSelect [value='" + data.IsAvailable + "']").attr("selected", "selected");
            currentBtns = ["#updateCoinBtn", "#deleteCoinBtn"];
            displayButton();
        },
        error: function(data) {
            $('#coinResult').css("display", "block");
            $('#coinResult').append(data.responseText);
        }
    });
}

//// AuthorName TextBox filling of selected in the ComboBox
//function setAuthorNameTextBoxFromComboBox() {
//    var select = document.getElementById("authorNameSelect");
//    select.addEventListener("click",function () {
//        $('#authorNameInput').val($('#authorNameSelect').val());
//        displayButton();
//    }, false);
//}

// subscription to change Author Name TextBox
function changeCoinNameTextBox() {
    var input = document.getElementById("coinName");
    input.addEventListener("change", function () {
        displayButton();
    }, false);
}

// subscription to change the content of a modal window
function keyUpDisplBtnForModalViewContent() {
    $('#content').keyup(function() { displayButton(); });
}

// subscription buttons to events
function btnEvents() {
    $("#createCoinBtn").bind("click", CreateCoin);
    $("#updateCoinBtn").bind("click", UpdateCoin);
    $("#deleteCoinBtn").bind("click", DeleteCoin);
}

function echo(data) {
    console.log(data);
}