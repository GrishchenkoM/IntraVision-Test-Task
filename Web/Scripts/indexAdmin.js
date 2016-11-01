/// <reference path="jquery-1.10.2.js" />
/// <reference path="~/Scripts/bootstrap.js" />

function coinTabEvents() {
        $("table[id=\"coinTab\"] tr").click(function () {
            if ($("td:first-child", this).html() != undefined)
                UpdateCoinView($("td:first-child", this).html());
        });
        $("table[id=\"coinTab\"] tbody tr").click(function () {
            $("table[id=\"coinTab\"] tbody tr").removeClass();
            $(this).addClass("selected");
        });
        $("table[id=\"coinTab\"] tbody tr").mouseover(function () {
            $("table[id=\"coinTab\"] tbody tr").removeClass();
            $(this).addClass("selected");
        });
        $("table[id=\"coinTab\"] tbody tr").mouseleave(function () {
            $("table[id=\"coinTab\"] tbody tr").removeClass();
        });
    }
function drinkTabEvents() {
    $("table[id=\"drinkTab\"] tr").click(function () {
        if ($("td:first-child", this).html() != undefined)
            UpdateDrinkView($("td:first-child", this).html());
    });
    $("table[id=\"drinkTab\"] tbody tr").click(function () {
        $("table[id=\"drinkTab\"] tbody tr").removeClass();
        $(this).addClass("selected");
    });
    $("table[id=\"drinkTab\"] tbody tr").mouseover(function () {
        $("table[id=\"drinkTab\"] tbody tr").removeClass();
        $(this).addClass("selected");
    });
    $("table[id=\"drinkTab\"] tbody tr").mouseleave(function () {
        $("table[id=\"drinkTab\"] tbody tr").removeClass();
    });
}

function hideCoinIdCol() {
    $('table[id="coinTab"] th:first-child').css("display", "none");
    $('table[id="coinTab"] td:first-child').css("display", "none");
}
function hideDrinkIdCol() {
    $('table[id="drinkTab"] th:first-child').css("display", "none");
    $('table[id="drinkTab"] td:first-child').css("display", "none");
}

function createThead() {
    var thead = document.createElement("thead");

    if (arguments.length > 0) {
        var tr = document.createElement("tr");

        for (var i = 0; i < arguments.length; ++i) {
            var th = document.createElement("th");
            th.appendChild(document.createTextNode(arguments[i]));
            tr.appendChild(th);
        }
        thead.appendChild(tr);
    }
    return thead;
}
function createCoinsTableBody(data) {
    function createCol(row, data) {
        var td = document.createElement("td");
        td.appendChild(document.createTextNode(data));
        row.appendChild(td);
    }

    var tbdy = document.createElement("tbody");

    for (var i = 0; i < data.length; ++i) {

        var tr = document.createElement("tr");
        tr.setAttribute("id", data[i].Id);

        createCol(tr, data[i].Id);
        createCol(tr, data[i].Name);
        createCol(tr, data[i].Number);

        if (data[i].IsAvailable)
            createCol(tr, "Yes");
        else
            createCol(tr, "No");
            
        tbdy.appendChild(tr);
    }
    return tbdy;
}
function createDrinksTableBody(data) {
    function createCol(row, data) {
        var td = document.createElement("td");
        td.appendChild(document.createTextNode(data));
        row.appendChild(td);
    }

    var tbdy = document.createElement("tbody");

    for (var i = 0; i < data.length; ++i) {

        var tr = document.createElement("tr");
        tr.setAttribute("id", data[i].Id);

        createCol(tr, data[i].Id);
        createCol(tr, data[i].Name);
        createCol(tr, data[i].Number);
        createCol(tr, data[i].Cost);

        tbdy.appendChild(tr);
    }
    return tbdy;
}

function displayCoins(data) {

    var div = $("#Coins");
    div.empty();

    var tbl = document.createElement("table");
    tbl.setAttribute("class", "table color_table");
    tbl.setAttribute("id", "coinTab");

    var thead = createThead("ID", "Name", "Number", "Is available");
        
    tbl.appendChild(thead);

    var tbdy = createCoinsTableBody(data);
        
    tbl.appendChild(tbdy);

    $("#Coins").append(tbl);

    coinTabEvents();

    hideCoinIdCol();
}
function displayDrinks(data) {
    var div = $("#Drinks");
    div.empty();

    var tbl = document.createElement("table");
    tbl.setAttribute("class", "table color_table");
    tbl.setAttribute("id", "drinkTab");

    var thead = createThead("ID", "Name", "Number", "Cost");

    tbl.appendChild(thead);

    var tbdy = createDrinksTableBody(data);

    tbl.appendChild(tbdy);
        
    $("#Drinks").append(tbl);

    drinkTabEvents();

    hideDrinkIdCol();
}

//Request to server
function Update(id) {
        
        //GetCoins
        if (id == null) {
            $.ajax({
                url: "/api/coin",
                type: "GET",
                dataType: "json",
                success: function (data) {
                    displayCoins(data);
                },
                error: function(data) {
                    var div = $("#Coins");
                    div.empty();
                    var substring = "Database is empty";
                    if (data.responseText.search(substring) != -1)
                        div.append(substring);
                }
            });
        } else {
            $.ajax({
                url: "/api/coin/",
                type: "GET",
                dataType: "json",
                data: { index: id },
                success: function (data) {
                    displayCoins(data);
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

        //GetDrinks
        $.ajax({
            url: "/api/drink",
            type: "GET",
            dataType: "json",
            success: function (data) {
                displayDrinks(data);
            },
            error: function(data) {
                var div = $("#Drinks");
                div.empty();
                var substring = "Database is empty";
                if (data.responseText.search(substring) != -1)
                    div.append(substring);
            }
        });
}
   
//Hide Modal view -> update content
function UpdateIndexContent(model, modDialogId) {

    if (modDialogId != null) {
        $(modDialogId).modal("hide");
    } else {
        $("#modDialog").modal("hide");
    }

    try {
        $(function() {
            Update();
        });
    } catch (e) {
        console.log(e);
    }
}

function CoinView() {
    $(function () {
        $.ajax({
            url: '/admin/CoinView',
            type: "POST",
            success: function (data) {
                $("#dialogCoinContent").html(data);
                $("#modCoinDialog").modal("show");
            }
        });
    });
}
function UpdateCoinView(id) {
    $(function () {
        $.ajax({
            url: '/admin/CoinView',
            type: "POST",
            data: { index: id },
            success: function (data) {
                $("#dialogCoinContent").html(data);
                $("#modCoinDialog").modal("show");
            }
        });
    });
}
function DrinkView() {
    $(function () {
        $.ajax({
            url: '/admin/DrinkView',
            type: "POST",
            success: function (data) {
                $("#dialogDrinkContent").html(data);
                $("#modDrinkDialog").modal("show");
            }
        });
    });
}
function UpdateDrinkView(id) {
    $(function () {
        $.ajax({
            url: '/admin/DrinkView',
            type: "POST",
            data: { index: id },
            success: function (data) {
                $("#dialogDrinkContent").html(data);
                $("#modDrinkDialog").modal("show");
            }
        });
    });
}

// subscription buttons to events
function btnEvents() {
    $("#createCoin").bind("click", CoinView);
    $("#createDrink").bind("click", DrinkView);
}