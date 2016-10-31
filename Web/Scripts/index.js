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
                //var div = $("#bookPageLinks");
                //div.empty();
                //if (data.PageInfo.TotalItems > data.PageInfo.PageSize)
                //    CreateBookPageLinks(data.PageInfo);
                //pageLinksStyle();
            },
            error: function (data) {
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
            //var div = $("#authorPageLinks");
            //div.empty();
            //if (data.PageInfo.TotalItems > data.PageInfo.PageSize)
            //    CreateAuthorPageLinks(data.PageInfo);
            //pageLinksStyle();
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

function displayCoins(data) {
    var coins = $("#Coins");
    coins.empty();

    var ul = document.createElement("ul");
    //ul.setAttribute("class", "table color_table");
    //ul.setAttribute("id", "bookTab");
    ul.setAttribute("class", "coin");

    for (var i = 0; i < data.length; ++i) {
        var li = document.createElement("li");
        //li.setAttribute("id", data.ContentModels[i].BookId);
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
    var drinks = $("#Drinks");
    drinks.empty();

    for (var i = 0; i < data.length; ++i) {
        var drink = document.createElement("div");

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

        drink.setAttribute("class", "product " + cellwidth);
        drink.setAttribute("id", data[i].Id);
        drink.appendChild(document.createTextNode(data[i].Name));
        drink.appendChild(document.createElement("br"));
        drink.appendChild(document.createTextNode(data[i].Cost));
        drink.appendChild(document.createElement("br"));

        var btn = document.createElement("input");
        btn.setAttribute("class", "drink_btn");
        //btn.setAttribute("class", "drink_btn_disable");
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

// subscription buttons to events
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
    echo(event.target.id.split('_')[1]);
    echo(event.target.value);
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
        //$("#btn_" + data[i].Id).removeClass("drink_btn_disable");
        //$("#btn_" + data[i].Id).addClass("drink_btn");
        $("#btn_" + data[i].Id).css("display", "block");
        $("#btn_" + data[i].Id).removeAttr("disabled");
        
        
    }
}

function ResetCoins() {
    ResetCoinsSum();
    ResetCoinArray();
    $(".row input[type=button]").attr("disabled", "disabled");
    $("#sum_lable").text("");
}


function echo(data) {
    console.log(data);
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
    echo("before hide");
    $("#modDialog_Change").modal("hide");
    ResetCoins();
    echo("after hide");
    try {
        $(function () {
            Update();
        });
    } catch (e) {
        console.log(e);
    }
}