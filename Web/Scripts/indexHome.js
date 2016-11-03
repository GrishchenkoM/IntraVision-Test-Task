/// <reference path="jquery-1.10.2.js" />
/// <reference path="bootstrap.js" />

//Request to server
function Update() {

    //GetCoins
    $.ajax({
            url: "/api/coin/",
            type: "GET",
            dataType: "json",
            success: function (data) {
                displayCoins(data);
            },
            error: function (data) {
                console.log(data);

                var div = $("#Coins");
                div.empty();
                var substring = "Database is empty";
                if (data.responseText.search(substring) != -1)
                    div.append(substring);
            }
        });

    //GetDrinks
    $.ajax({
        url: "/api/drink/",
        type: "GET",
        dataType: "json",
        success: function (data) {
            displayDrinks(data);
        },
        error: function (data) {
            console.log(data);

            var div = $("#Drinks");
            div.empty();
            var substring = "Database is empty";
            if (data.responseText.search(substring) != -1)
                div.append(substring);
        }
    });
}

function displayCoins(data) {
    var coins = $("#Coins");
    coins.empty();

    var ul = document.createElement("ul");
    ul.setAttribute("class", "coin");

    for (var i = 0; i < data.length; ++i) {
        var li = document.createElement("li");
        var btn = document.createElement("input");
        btn.setAttribute("type", "button");
        btn.setAttribute("class", "coin_btn");
        btn.setAttribute("value", data[i].Name);

        if (!data[i].IsAvailable)
            btn.setAttribute("class", "disable_btn");

        li.appendChild(btn);
        ul.appendChild(li);
    }

    $("#Coins").prepend(ul);

    $(".coin_btn").bind("click", TookCoin);
}
function displayDrinks(data) {
    function createDiv(id, data) {
        var div = document.createElement("div");
        div.setAttribute("id", id);

        if (data != undefined)
            div.appendChild(document.createTextNode(data));

        return div;
    }

    var drinks = $("#Drinks");
    drinks.empty();

    for (var i = 0; i < data.length; ++i) {
        var drink = document.createElement("div");
        drink.setAttribute("id", "drink_" + data[i].Id);

        var cellwidth;
        switch (5) {
            case 1:
                cellwidth = "col-lg-12 col-md-12 col-sm-12 col-xs-12";
            case 2:
                cellwidth = "col-lg-6 col-md-6 col-sm-6 col-xs-12";
            break;
            case 3:
                cellwidth = "col-lg-4 col-md-4 col-sm-6 col-xs-12";
            break;
            case 4:
                cellwidth = "col-lg-3 col-md-3 col-sm-4 col-xs-12";
            break;
            case 5:
                cellwidth = "col-lg-3 col-md-3 col-sm-4 col-xs-12";
            break;
            default:
                cellwidth = "col-lg-2 col-md-3 col-sm-4 col-xs-12";
        }
        drink.setAttribute("class", "product " + cellwidth + " cust");
        
        var div = createDiv("nameOfDrink", data[i].Name);
        div.className = "h4";
        drink.appendChild(div);

        div = createDiv("costOfDrink", data[i].Cost);
        drink.appendChild(div);

        div = createDiv("errorMessage", data[i].ErrorMessage);
        div.className = "h4";
        drink.appendChild(div);

        var btn = document.createElement("input");
        btn.className = "drink_btn btn-danger";
        btn.setAttribute("disabled", "disabled");
        btn.setAttribute("id", "btn_" + data[i].Id);
        btn.setAttribute("type", "button");
        btn.setAttribute("value", "Buy it!");
        
        drink.appendChild(btn);
        drinks.append(drink);
    }
    
    $("#Drinks").append(drinks);
    $(".drink_btn").bind("click", MakePurchase);
}

function btnEvents() {
    $("#cancel_btn").bind("click", ResetCoins);
}

function TookCoin() {
    var coin = parseInt(this.value);
    AddCoin(coin);
    $("#sum_lable").text(GetSum());
    AddCoinToArray(coin);
    var arr = GetCoinArray();
    $.ajax({
        url: "/home/GetDrinksBySum",
        type: "POST",
        data: {coins: arr},
        success: function(data) {
            EnableBuyButtons(data);
        },
        error: function (data) {
            var div = $("#Coins");
            div.empty();
            var substring = "Database is empty";
            if (data.responseText.search(substring) != -1)
                div.append(substring);
        }
    });
}
function MakePurchase() {
    var arr = GetCoinArray();
    $.ajax({
        url: "/home/MakePurchase/" + event.target.id.split('_')[1],
        type: "POST",
        data: { coins: arr },
        success: function (data) {
            $("#changeContentDialog").html(data);
            $("#drinkName_lbl").val(event.target.value);
            $("#clientSum_lbl").val(GetSum());
            $("#modDialog_Change").modal("show");
        },
        error: function (data) {
            var div = $("#Drinks");
            div.empty();
            var substring = "Database is empty";
            if (data.responseText.search(substring) != -1)
                div.append(substring);
        }
    });
}

function EnableBuyButtons(data) {
    for (var i = 0; i < data.length; ++i) {
        if (data[i].ErrorMessage !== undefined && data[i].ErrorMessage !== null) {
            $("#drink_" + data[i].Id + " #errorMessage").text(data[i].ErrorMessage);
        } else {
            $("#btn_" + data[i].Id).css("display", "block");
            $("#btn_" + data[i].Id).removeAttr("disabled");
            $("#btn_" + data[i].Id).removeClass("drink_btn btn-danger");
            $("#btn_" + data[i].Id).addClass("drink_btn btn-success");
            $("#drink_" + data[i].Id + " #errorMessage").text("");
        }
    }
}
function ResetCoins() {
    ResetCoinsSum();
    ResetCoinArray();
    $(".row input[type=button]").attr("disabled", "disabled");
    $("#sum_lable").text("");
    Update();
}

function makeSumCounter() {
    var sum = 0;
    this.AddCoin = function (coin) { sum += coin; }
    this.ResetCoinsSum = function () { sum = 0; }
    this.GetSum = function () { return sum; }
}
function makeCoinArray() {
    var arr = [];
    
    this.AddCoinToArray = function (coin) { arr.push(coin); }
    this.GetCoinArray = function () { return arr; }
    this.ResetCoinArray = function () { arr = []; }
}

//Hide Modal view -> update content
function UpdateIndexContent() {
    $("#modDialog_Change").modal("hide");
    ResetCoins();
    try {
        $(function () {
            Update();
        });
    } catch (e) {
        console.log(e);
    }
}