const guestName = document.getElementById("guestName");
const cardNum = document.getElementById("cardNum");
const expiryDate = document.getElementById("expiryDate");
const cardCvv = document.getElementById("cardCvv");

const hotelPreviewName = document.getElementById("hotelPreviewName");
const hotelPreviewNumber = document.getElementById("hotelPreviewNumber");
const hotelPreviewExpiry = document.getElementById("hotelPreviewExpiry");
const hotelPreviewCvv = document.getElementById("hotelPreviewCvv");
const hotelCardPreview = document.getElementById("hotelCardPreview");

guestName.addEventListener("input", () => {
    hotelPreviewName.textContent = guestName.value ? guestName.value.toUpperCase() : "GUEST NAME";
    hotelCardPreview.classList.add("active");
});

cardNum.addEventListener("input", (e) => {
    let digits = e.target.value.replace(/\D/g, "").slice(0, 16);
    let display = digits.replace(/(\d{4})(?=\d)/g, "$1 ").trim();
    cardNum.value = display;
    hotelPreviewNumber.textContent = display || "**** **** **** ****";
});

expiryDate.addEventListener("input", (e) => {
    let d = e.target.value.replace(/\D/g, "").slice(0, 4); // MMYY
    if (d.length >= 2) {
        let mm = parseInt(d.slice(0, 2), 10);
        if (mm <= 0) mm = 1;
        if (mm > 12) mm = 12;
        d = mm.toString().padStart(2, "0") + d.slice(2);
    }
    let display = d.length > 2 ? d.slice(0, 2) + "/" + d.slice(2) : d;
    expiryDate.value = display;
    hotelPreviewExpiry.textContent = display || "MM/YY";
});

cardCvv.addEventListener("focus", () => {
    hotelCardPreview.classList.add("show-back");
});
cardCvv.addEventListener("blur", () => {
    hotelCardPreview.classList.remove("show-back");
});
cardCvv.addEventListener("input", () => {
    hotelPreviewCvv.textContent = cardCvv.value ? cardCvv.value.replace(/./g, "•") : "***";
});
