function initializeFormTransactions(obtainCategoriesUrl) {
    $("#TypeOperationId").change(async function () {
        const selectedValue = $(this).val();

        const response = await fetch(obtainCategoriesUrl, {
            method: 'POST',
            body: selectedValue,
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const json = await response.json();
        const options = json.map(category => `<option value=${category.value}>${category.text}</option>`);
        $("#CategoryId").html(options);
    })
}
