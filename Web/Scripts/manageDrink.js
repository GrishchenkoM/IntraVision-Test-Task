
var currentBtns = []; // display list certain keys this modal window

var content = {}; // window content storage

// content reading
function readContent() {
    var drinkId = $('#drinkId').val();
    if (drinkId != '') drinkId = parseInt($('#drinkId').val());
    else drinkId = -1;

    content.drinkId = drinkId;
    content.drinkName = $('#drinkName').val().replace(new RegExp('\n', 'g'), '');
    content.drinkNumber = $('#drinkNumber').val();
    content.drinkCost = $('#drinkCost').val();
}

// drink creation - POST
function CreateDrink() {
        
    readContent();
        
    var viewModel = {
        Id: content.drinkId,
        Name: content.drinkName,
        Number: content.drinkNumber,
        Cost: content.drinkCost
    }

    $.ajax({
        url: '/api/drink/',
        type: 'POST',
        data: viewModel,
        success: function(model) {
            window.UpdateIndexContent(model, '#modDrinkDialog');
        },
        error: function(data) {
            $('#drinkResult').css("display", "block");
            $('#drinkResult').append(data.responseText);
        }
    });
}

// modification of drink - PUT
function UpdateDrink() {

    readContent();

    var viewModel = {
        Id: content.drinkId,
        Name: content.drinkName,
        Number: content.drinkNumber,
        Cost: content.drinkCost
    }

    $.ajax({
        url: '/api/drink/',
        type: 'PUT',
        data: viewModel,
        success: function(model) {
            window.UpdateIndexContent(model, '#modDrinkDialog');
        },
        error: function(data) {
            $('#drinkResult').css("display", "block");
            $('#drinkResult').append(data.responseText);
        }
    });
}

// deleting of drink - DELETE
function DeleteDrink() {
        
    readContent();

    $.ajax({
        url: '/api/drink/' + content.drinkId,
        type: 'DELETE',
        success: function(model) {
            window.UpdateIndexContent(model, '#modDrinkDialog');
        },
        error: function(data) {
            $('#drinkResult').css("display", "block");
            $('#drinkResult').append(data.responseText);
        }
    });
}

// button on modal view. Shows when all form elements are filled
function displayButton() {
    var name = $('#drinkName').val().replace(new RegExp('\n', 'g'), '').replace(new RegExp(' ', 'g'), '');
    var number = $('#drinkNumber').val().replace(new RegExp('\n', 'g'), '').replace(new RegExp(' ', 'g'), '');
    var cost = $('#drinkCost').val().replace(new RegExp('\n', 'g'), '').replace(new RegExp(' ', 'g'), '');
    if (name !== '' && number !== '' && cost !== '') {
        for (var i = 0; i < currentBtns.length; ++i) {
            //$('#drinkResult').css("display", "none");
            $(currentBtns[i]).css("display", "block");
        }
    } else {
        for (var i = 0; i < currentBtns.length; ++i) {
            $(currentBtns[i]).css("display", "none");
            $('#drinkResult').css("display", "block");
        }
    }
}

// request for info about the drink - GET
function getDrink(model) {
    $.ajax({
        url: '/api/drink/' + model,
        type: 'GET',
        //data: { index: model },
        dataType: 'json',
        success: function(data) {
            $('#drinkId').val(data.Id);
            $('#drinkName').val(data.Name);
            $('#drinkNumber').val(data.Number);
            $('#drinkCost').val(data.Cost);
            currentBtns = ["#updateDrinkBtn", "#deleteDrinkBtn"];
            displayButton();
        },
        error: function(data) {
            $('#drinkResult').css("display", "block");
            $('#drinkResult').append(data.responseText);
        }
    });
}

// subscription to change Drink Name TextBox
function changeDrinkNameTextBox() {
    var input = document.getElementById("drinkName");
    input.addEventListener("change", function () {
        displayButton();
    }, false);
}

// subscription to change the content of a modal window
function keyUpDisplBtnForModalViewContent() {
    $('#content').keyup(function() { displayButton(); });
    //$('#drinkName').keyup(function() { displayButton(); });
}

// subscription buttons to events
function btnEvents() {
    $("#createDrinkBtn").bind("click", CreateDrink);
    $("#updateDrinkBtn").bind("click", UpdateDrink);
    $("#deleteDrinkBtn").bind("click", DeleteDrink);
}

