﻿var portfolioManager = function() {

    // appends a row to the portfolio items table.
    // @parentSelector: selector to append a row to.
    // @obj: portfolio item object to append.
    var appendRow = function (parentSelector, obj) {
        var price;
        var stock = $.getJSON("/api/Stock/" + obj.Symbol, function (data) {
            var sum, price;
            if (data !== null) {
                price = data.Price.toFixed(2);
                sum = data.Price * obj.SharesNumber;
                sum = sum.toFixed(2);
            }
            else {
                price = "-";
                sum = "-"
            }
            var tr = $("<tr data-id='" + obj.ItemId + "'></tr>");
            tr.append("<td class='name' >" + obj.Symbol + "</td>");
            tr.append("<td class='name' >" + obj.SharesNumber + "</td>");
            tr.append("<td class='name' >" + price + "</td>");
            tr.append("<td class='name' >" + sum + "</td>");
            tr.append("<td><button class='btn btn-warning btn-sm'><i class='glyphicon glyphicon-pencil'> Update</i></button><button class='btn btn-danger btn-sm' style='margin-left:2%'><i class='glyphicon glyphicon-remove'> Delete</i></button></td>");

            $(parentSelector).append(tr);
        });
    }

    // adds all portfolio items as rows (deletes all rows before).
    // @parentSelector: selector to append a row to.
    // @tasks: array of portfolio items to append.
    var displayPortfolioItems = function(parentSelector, portfolioItems) {
        $(parentSelector).empty();
        $.each(portfolioItems, function (i, item) {
            appendRow(parentSelector, item);
        });
    };

    // starts loading portfolio items from server.
    // @returns a promise.
    var loadPortfolioItems = function() {
        return $.getJSON("/api/portfolioitems");
    };

    // starts creating a portfolio item on the server.
    // @symbol: symbol name.
    // @sharesNumber: number of shares.
    // @return a promise.
    var createPortfolio = function(symbol, sharesNumber) {
        return $.post("/api/portfolioitems",
        {
            Symbol: symbol,
            SharesNumber: sharesNumber
        });
    };

    // starts updating a portfolio item on the server.
    // @id: id of the portfolio item to update.
    // @symbol: symbol name.
    // @sharesNumber: number of shares.
    // @return a promise.
    var updatePortfolioItem = function(id, symbol, sharesNumber) {
        return $.ajax(
        {
            url: "/api/portfolioitems",
            type: "PUT",
            contentType: 'application/json',
            data: JSON.stringify({
                ItemId: id,
                Symbol: symbol,
                SharesNumber: sharesNumber
            })
        });
    };

    // starts deleting a portfolio item on the server.
    // @itemId: id of the item to delete.
    // @return a promise.
    var deletePortfolioItem = function (itemId) {
        return $.ajax({
            url: "/api/portfolioitems/" + itemId,
            type: 'DELETE'
        });
    };

    // returns public interface of portfolio manager.
    return {
        loadItems: loadPortfolioItems,
        displayItems: displayPortfolioItems,
        createItem: createPortfolio,
        deleteItem: deletePortfolioItem,
        updateItem: updatePortfolioItem
    };
}();


$(function () {
    // add new portfolio item button click handler
    $("#newCreate").click(function() {
        var symbol = $('#symbol')[0].value;
        var sharesNumber = $('#sharesNumber')[0].value;

        portfolioManager.createItem(symbol, sharesNumber)
            .catch(function () {
                alert('You already have an item with same symbol!')
            })
            .then(portfolioManager.loadItems)
            .done(function(items) {
                portfolioManager.displayItems("#items > tbody", items);
            });
    });

    // bind update portfolio item checkbox click handler
    $("#items > tbody").on('click', '.btn-warning', function () {
        var tr = $(this).parent().parent();
        var itemId = tr.attr("data-id");
        var symbol = $('#symbol')[0].value;
        var sharesNumber = $('#sharesNumber')[0].value;
        //var symbol = tr.find('.symbol').text();
        //var sharesNumber = tr.find('.sharesNumber').text();
        
        portfolioManager.updateItem(itemId, symbol, sharesNumber)
            .then(portfolioManager.loadItems)
            .done(function (items) {
                portfolioManager.displayItems("#items > tbody", items);
            });
    });

    // bind delete button click for future rows
    $('#items > tbody').on('click', '.btn-danger', function () {
        var itemId = $(this).parent().parent().attr("data-id");
        portfolioManager.deleteItem(itemId)
            .then(portfolioManager.loadItems)
            .done(function(items) {
                portfolioManager.displayItems("#items > tbody", items);
            });
    });

    // load all items on startup
    portfolioManager.loadItems()
        .done(function(items) {
            portfolioManager.displayItems("#items > tbody", items);
        });
});