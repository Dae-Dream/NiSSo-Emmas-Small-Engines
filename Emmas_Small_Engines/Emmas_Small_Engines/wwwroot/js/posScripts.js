//The tracker variable is the table body, it tracks the number of child elements (Cart Items) it has
let tracker = document.getElementById("tableTracker");
//This helps to auto generate event listeners for created increase and reduce quantity buttons
//Now each cart item has its own unique increase/decrease buttons and quantity input fields that auomatically updates the inventory stock
for (let i = 0; i < tracker.childElementCount; i++) {
	document.getElementById(`increaseQuantity-${i}`).addEventListener("click", function () {
		document.getElementById(`quantity-${i}`).value++;		
	})
	document.getElementById(`reduceQuantity-${i}`).addEventListener("click", function () {
		document.getElementById(`quantity-${i}`).value--;		
	})
}

