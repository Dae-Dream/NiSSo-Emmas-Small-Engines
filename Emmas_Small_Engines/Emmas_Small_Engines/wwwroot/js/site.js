// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let navItems = document.getElementsByTagName('li');
let navLinks = document.getElementsByClassName('nav-link');
let links = document.getElementsByTagName('a');
//lastActive.classList.add('active');
for (let i = 1; i < navLinks.length; i++) {
   
    //Event Listener adding rounded background to all currently inactive nav links when the cursor hovers over the element.
    navLinks[i].addEventListener('mouseover', function () { //Mouse over effect on navigation links
        if (navLinks[i].classList.contains('active')) {
            return
        }
        navLinks[i].classList.add('bg-dark');
        navLinks[i].classList.add('bg-gradient');
        navLinks[i].classList.add('rounded-2');
    })
    //Event that removes rounded background when the cursor stops hovering over the element
    navLinks[i].onmouseleave = function () {
        if (navLinks[i].classList.contains('active')) {
            return
        }
        navLinks[i].classList.remove('bg-dark');
        navLinks[i].classList.remove('bg-gradient');
        navLinks[i].classList.remove('rounded-2');

    }
}

