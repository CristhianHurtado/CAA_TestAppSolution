$("#form").submit(function (event) {

    var dictionary = {};
    var errors = [];

    $("#product-table tbody tr.filterable:visible").each(function () {
        var productId = $(this).attr("data-id");
        var location = $(this).find("input[name='locations']").val();
        var quantity = $(this).find("input[name='quan']").val();
        var availableQuantity = parseInt($(this).find("td:eq(1)").text());

        if (!dictionary[productId]) {
            dictionary[productId] = [[location, quantity]];
        } else {
            dictionary[productId].push([location, quantity]);
        }

        if (parseInt(quantity) > availableQuantity) {
            $('#span-header').removeAttr('hidden');
            $(this).find(".alert-span").remove();
            $('#span-cell').remove();
            event.preventDefault();
            errors.push(`Quantity of ${quantity} exceeds available quantity of ${availableQuantity} for product ${productId}`);
            if ($(this).find(".alert-span").length === 0) {
                $(this).append(`<td id="span-cell"><span class="alert-span" style="color:red;">Quantity exceeds available quantity</span></td>`);
            }
        }
        else if (parseInt(quantity) < 0) {
            $('#span-header').removeAttr('hidden');
            $(this).find(".alert-span").remove();
            $('#span-cell').remove();
            event.preventDefault();
            errors.push(`Quantity can't be negative'`);
            if ($(this).find(".alert-span").length === 0) {
                $(this).append(`<td id="span-cell"><span class="alert-span" style="color:red;">Quantity taken cannot be negative</span></td>`);
            }
        }
        else {
            $(this).find(".alert-span").remove();
            $('#span-header').remove();
            $('#span-cell').remove();
        }
    });

    $('.hidden-dataInfo-input').val(JSON.stringify(dictionary));
});