
//---------DOM ELEMENTS FOR CREATE PAGE-----------------
let DDLforChosen = document.getElementById("selectedOptions");
let OLforChosen = document.getElementById("orderList");
let DDLforAvail = document.getElementById("availOptions");
let invenCards_li = document.getElementsByName("testLi");
let invenCards_checks = document.getElementsByName("test_check");
let DDLforAmount = document.getElementById("orderAmount");
//-----------------------------------------------------
//------------EVENT-LISTENERS FOR CREATE PAGE---------------------

//Button to add an item to the order list
document.getElementById("btnRight").addEventListener("click", function () {
    for (let i = 0; i < DDLforAvail.children.length; i++){
        if (invenCards_checks[i].checked == true) {
            document.getElementById(`orderAmount-${invenCards_li[i].id}`).classList.toggle("d-none");
            document.getElementById(`orderAmt-${invenCards_li[i].id}`).classList.toggle("d-none");
            OLforChosen.insertBefore(invenCards_li[i], OLforChosen.children[parseInt(invenCards_li[i].id)]);
            
            
        }     
    }
   
    invenCards_checks.forEach(function (o) {
        o.checked = false;
    })
})

//button to remove an item from the order list
document.getElementById("btnLeft").addEventListener("click", function () {
    for (let o = 0; o < invenCards_li.length; o++) {
        if (invenCards_checks[o].checked == true) { 
            document.getElementById(`orderAmount-${invenCards_li[o].id}`).classList.toggle("d-none");
            document.getElementById(`orderAmt-${invenCards_li[o].id}`).classList.toggle("d-none");
            DDLforAvail.insertBefore(invenCards_li[o], DDLforAvail.children[parseInt(invenCards_li[o].id)]);

        }
    }
    invenCards_checks.forEach(function (o) {
        o.checked = false;
    })

})

//Create Order Request button
document.getElementById("btnSubmit").addEventListener("click", function () {
    for (let u = 0; u < OLforChosen.children.length; u++) {

        let quantity = document.getElementById(`orderAmount-${u}`).value;
        DDLforChosen.innerHTML += `<option value="${OLforChosen.children[u].value}" id="${OLforChosen.children[u].value}"></option>`;

        DDLforAmount.innerHTML += `<option value="${OLforChosen.children[u].value}-${quantity}" id="${OLforChosen.children[u].value}-${quantity}"></option>`;
        //alert(DDLforAmount.children.value);
    }
    DDLforChosen.childNodes.forEach(opt => opt.selected = "selected");
    DDLforAmount.childNodes.forEach(opt => opt.selected = "selected");
})

//Search bar functionality
let searchString = document.getElementById("searchString");
let searchButton = document.getElementById("searchBtn");
let searchOptions = document.getElementById("searchOptions");
let suggestions = new Set();
window.onload = function () {
    if (searchString.value == "" || null) {
        searchOptions.innerHTML = "";
        searchOptions.classList.add("d-none");
    } else {
        searchOptions.classList.remove("d-none");
    }
};
//Search bar text input event listener
//Searches for matching item names on input and shows suggestions in drop-down
//Clicking on a drop down items name scrolls to the item
searchString.addEventListener("input", function () {
    let invenCards = document.getElementsByClassName("card");
    if (searchString.value == "" || null) {
        searchOptions.innerHTML = "";
        searchOptions.classList.add("d-none");
    } else {
        for (let c = 0; c < invenCards.length; c++) {
            if (invenCards[c].id.toLowerCase() == searchString.value.toLowerCase()) {
                searchOptions.classList.remove("d-none");
                searchOptions.innerHTML = `<li class="bg-light w-100">
                                                <button type="button"  name="suggestionLink" onmouseover="Selected()" onclick="ScrollTo('${invenCards[c].id}')" class="btn text-start w-100 btn-link text-secondary text-decoration-none">${invenCards[c].id}</button>
                                            </li>`;
            }
            else if (invenCards[c].id.toLowerCase().includes(searchString.value.toLowerCase())) {
                suggestions.add(invenCards[c].id)
                searchOptions.classList.add("d-none");
            } else if (!invenCards[c].id.toLowerCase().includes(searchString.value.toLowerCase())) {
                suggestions.delete(invenCards[c].id);
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