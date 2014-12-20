/// <reference path="knockout-3.2.0.js" />


function ExpenseGroupsModel() {

    var self = this;

    self.expenseGroups = ko.observableArray([]);

    self.getUserExpenseGroups = function getUserExpenseGroups() {
        $.ajax("/Home/UserExpenseGroups",
            { async: false })
            .then(function (result) {
                self.expenseGroups(result);
            });
    }

    return self;
}

function LineItemViewModel(description, expenseType, taxCode, amount, glCodes) {
    this.description = ko.observable(description);
    this.expenseType = ko.observable(expenseType);
    this.taxCode = ko.observable(taxCode);
    this.amount = ko.observable(amount);
    this.glCodes = ko.observableArray(glCodes);
}

function GlCodeViewModel(segmentId, code, description) {
    this.segmentId = ko.observable(segmentId);
    this.code = ko.observable(code);
    this.description = ko.observable(description);
}

function ExpenseViewModel() {

    var self = this;

    self.transactionDate = ko.observable();
    self.merchantName = ko.observable();
    self.amount = ko.observable();
    self.purpose = ko.observable("");
    self.expenseGroup = ko.observable("");
    self.taxReceipt = ko.observable(false);
    self.lineItems = ko.observableArray();

    self.selectedLineItemIndex = ko.observable(0);
    self.selectedLineItem = ko.observable();
    self.selectedGlCode = ko.observable();

    self.expenseTypes = ko.observableArray();
    self.taxCodes = ko.observableArray();

    self.load = function () {
        var model = getTransactionInfo();

        self.transactionDate(model.Transaction.TransactionDate);
        self.merchantName(model.Transaction.MerchantName);
        self.amount(model.Transaction.Amount);
        self.purpose(model.Transaction.Purpose);
        self.expenseGroup(model.Transaction.ExpenseGroup);
        self.taxReceipt(model.Transaction.TaxReceipt);

        self.expenseTypes(model.ExpenseTypeDetails);
        self.taxCodes(model.TaxCodeDetails);

        model.LineItems.forEach(function (item) {

            var glCodes = item.GlCodes.map(function (gl) {
                return new GlCodeViewModel(gl.SegmentId, gl.Code, gl.Name);
            });

            self.lineItems.push(new LineItemViewModel(
                item.Description,
                item.ExpenseType,
                item.TaxCode,
                item.Price,
                glCodes));
        });

        self.selectedLineItem(self.lineItems()[self.selectedLineItemIndex()]);
    };

    self.currentLineNumber = ko.computed(function () {
        return self.selectedLineItemIndex() + 1;
    });

    self.selectLineItem = function () {
        self.selectedLineItem(self.lineItems()[self.selectLineItemIndex()]);
    }

    self.isNextAvailable = function () {
        return self.lineItems().length > 0 && self.selectedLineItemIndex() + 1 < self.lineItems().length;
    }

    self.isPrevAvailable = function () {
        return self.lineItems().length > 1 && self.selectedLineItemIndex() > 0;
    }

    self.next = function () {
        self.selectedLineItemIndex(self.selectedLineItemIndex() + 1);
        self.selectedLineItem(self.lineItems()[self.selectedLineItemIndex()]);
    };

    self.prev = function () {
        self.selectedLineItemIndex(self.selectedLineItemIndex() - 1);
        self.selectedLineItem(self.lineItems()[self.selectedLineItemIndex()]);
    };

    self.showPageNavigation = function () {
        return self.lineItems().length > 1
    };

    self.showExpenseGroupModal = function () {
        var data = getUserExpenseGroups();
        $('#expenseGroupModal').modal();
    };

    self.showExpenseTypeModal = function () {
        $('#expenseTypeModal').modal();
    }

    self.setExpenseTypeSelected = function () {
        self.selectedLineItem().expenseType(this.ExpenseType);
        $('#expenseTypeModal').modal("hide");
    }

    self.showTaxCodeModal = function () {
        console.log("clicked taxCode");
        $('#taxCodeModal').modal();
    }

    self.setTaxCodeSelected = function () {
        self.selectedLineItem().taxCode(this.TaxCode);
        $('#taxCodeModal').modal("hide");
    }

    self.showGlCodeModal = function (element, allBindings, viewModel) {
        console.log("clicked glCode, segment=" + viewModel.segmentId());
        self.selectedGlCode(viewModel);
        $('#glCodeModal h4').text(viewModel.description() + " GL Search");
        $('#glCodeModal').modal();
    }

    self.getGlSearchResults = function () {
        var url = "Home/GlSearch";
        var segmentId = self.selectedGlCode().segmentId();

        console.log("segmentId=" + segmentId);

        $.ajax({
            type: "GET",
            url: url,
            data: { segmentId: segmentId },
            dataType: "json"
        }).then(function (result) {
            console.log(result.length);
        });
    }

    function getTransactionInfo() {

        return model;
        var data = {};

        $.ajax("/Home/GetExpenseDetailsViewModel",
            { async: false })
            .then(function (result) {
                data = result;
            });

        return data;
    }
}

var expenseViewModel = new ExpenseViewModel();
var expenseGroupsViewModel = new ExpenseGroupsModel();

$(function () {

    ko.bindingHandlers.modalex = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var setupFunction = valueAccessor();
            $(element).on("click", function () { setupFunction(element, allBindings, viewModel); });
        }
    };

    ko.applyBindings(expenseViewModel, document.getElementById("content"));
    ko.applyBindings(expenseGroupsViewModel, document.getElementById("expenseGroupModal"));

    expenseViewModel.load();

});
