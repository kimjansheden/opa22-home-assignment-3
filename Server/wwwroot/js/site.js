document.addEventListener("DOMContentLoaded", function() {
    const buyOrSellSelect = document.getElementById("BuyOrSell");
    const buyAdFields = document.querySelectorAll(".buyAd");
    const sellAdFields = document.querySelectorAll(".sellAd");

    function updateAdFields() {
        const buyOrSellValue = buyOrSellSelect.value;
        for (const field of buyAdFields) {
            field.style.display = buyOrSellValue === "1" ? "" : "none";
        }
        for (const field of sellAdFields) {
            field.style.display = buyOrSellValue === "2" ? "" : "none";
        }
    }

    if (buyOrSellSelect) {
        buyOrSellSelect.addEventListener("change", updateAdFields);
        updateAdFields();
    }
});
