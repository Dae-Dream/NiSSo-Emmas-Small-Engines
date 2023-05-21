//document.getElementById("btnFill").addEventListener("click", function () {
    //document.getElementById("ExternalOrderNumber").value = "EM-9901";

    //.getElementById("Description").value = "Parts for building";
//})
//---------DOM ELEMENTS FOR EDIT PAGE-----------------
let DDLforChosenEdit = document.getElementById("selectedOptions");
let OLforChosenEdit = document.getElementById("orderListEdit");
let DDLforAvailEdit = document.getElementById("availableOptionsEdit");
let invenCards_li_Edit = document.getElementsByName("invenCards_li_Edit");
let invenCards_checks_Edit = document.getElementsByName("invenCards_checks_Edit");
let DDLforAmount = document.getElementById("orderAmount");

//----------------------------------------------------

//--------------EVENT LISTENERS FOR EDIT PAGE-----------------------
document.getElementById("btnRightEdit").addEventListener("click", function () {
    for (let a = 0; a < DDLforAvailEdit.children.length; a++) {
        if (invenCards_checks_Edit[a].checked == true) {
            document.getElementById(`orderAmt-${invenCards_li_Edit[a].id}`).classList.toggle("d-none");
            document.getElementById(`orderAmount-${invenCards_li_Edit[a].id}`).classList.toggle("d-none");
            OLforChosenEdit.insertBefore(invenCards_li_Edit[a], OLforChosenEdit.children[parseInt(invenCards_li_Edit[a].id)]);

            //alert(invenCards_li_Edit[a].id);
        }
    }

    invenCards_checks_Edit.forEach(function (c) {
        c.checked = false;
    })
})
document.getElementById("btnLeftEdit").addEventListener("click", function () {
    for (let o = 0; o < invenCards_li_Edit.length; o++) {
        if (invenCards_checks_Edit[o].checked == true) {
            document.getElementById(`orderAmt-${invenCards_li_Edit[o].id}`).classList.toggle("d-none");
            document.getElementById(`orderAmount-${invenCards_li_Edit[o].id}`).classList.toggle("d-none");
            DDLforAvailEdit.insertBefore(invenCards_li_Edit[o], DDLforAvailEdit.children[parseInt(invenCards_li_Edit[o].id)]);


        }
    }
    invenCards_checks_Edit.forEach(function (o) {
        o.checked = false;
    })

})
document.getElementById("btnSubmitEdit").addEventListener("click", function () {

    for (let u = 0; u < OLforChosenEdit.children.length; u++) {
        let quantity = document.getElementById(`orderAmount-${u}`).value;

        DDLforChosenEdit.innerHTML += `<option value="${OLforChosenEdit.children[u].value}" id="${OLforChosenEdit.children[u].value}" ></option>`;
        DDLforAmount.innerHTML += `<option value="${OLforChosenEdit.children[u].value}-${quantity}" id="${OLforChosenEdit.children[u].value}-${quantity}"></option>`;

       // alert(OLforChosenEdit.children[u].value)
    }
    DDLforChosenEdit.childNodes.forEach(opt => opt.selected = "selected");
    DDLforAmount.childNodes.forEach(opt => opt.selected = "selected");

})
//Edit Page search-bar functionality
let searchString = document.getElementById("searchStringEdit");
let searchButton = document.getElementById("searchBtn");
let searchOptions = document.getElementById("searchOptionsEdit");
let suggestions = new Set();
window.onload = function () {
    if (searchString.value == "" || null) {
        searchOptions.innerHTML = "";
        searchOptions.classList.add("d-none");
    } else {
        searchOptions.classList.remove("d-none");
    }
};
//Searches for matching item names on input and shows suggestions in drop-down
//Clicking on a drop down items name scrolls to the item
searchString.addEventListener("input", function () {
    let invenCards = document.getElementsByClassName("card");
    if (searchString.value == "" || null) {
        searchOptions.innerHTML = "";
        searchOptions.classList.add("d-none");
    } else {
        for (let ce = 0; ce < invenCards.length; ce++) {
            if (invenCards[ce].id.toLowerCase() == searchString.value.toLowerCase()) {
                searchOptions.classList.remove("d-none");
                searchOptions.innerHTML = `<li class="bg-light w-100">
                                                <button type="button"  name="suggestionLink" onmouseover="Selected()" onclick="ScrollTo('${invenCards[ce].id}')" class="btn text-start w-100 btn-link text-secondary text-decoration-none">${invenCards[ce].id}</button>
                                            </li>`;
            }
            else if (invenCards[ce].id.toLowerCase().includes(searchString.value.toLowerCase())) {
                suggestions.add(invenCards[ce].id)
                searchOptions.classList.add("d-none");
            } else if (!invenCards[ce].id.toLowerCase().includes(searchString.value.toLowerCase())) {
                suggestions.delete(invenCards[ce].id);
                searchOptions.classList.add("d-none");
            } else {
                searchOptions.innerHTML = "";
                searchOptions.classList.add("d-none");
            }

        }
        ShowSuggestions(suggestions.values());
    }

})


//Scrolls to the matching inventory item
function ScrollTo(a) {
    searchOptions.classList.add("d-none");
    searchOptions.innerHTML = "";
    searchString.value = a;
    document.getElementById(a).scrollIntoView();
}
function Selected() {
    let suggestionLinks = document.getElementsByName("suggestionLink")
    for (const link of suggestionLinks) {
        link.addEventListener("mouseenter", function () {
            link.classList.add("bg-primary");
            link.classList.add("text-white");

        })
        link.addEventListener("mouseleave", function () {
            link.classList.remove("bg-primary");
            link.classList.remove("text-white");
        })
    }
}
//Shows search suggestions in dropdown div
function ShowSuggestions(suggestion) {
    let text = "";
    for (const item of suggestion) {

        text += `<li class="bg-light w-100" >
                                        <button type="button" name="suggestionLink" onmouseover="Selected()" onclick="ScrollTo('${item}')" class="btn w-100 text-start btn-link text-secondary text-align-left text-decoration-none">${item}</button>
                                    </li>`;

    }
    searchOptions.innerHTML = text;
    searchOptions.classList.remove("d-none");
}