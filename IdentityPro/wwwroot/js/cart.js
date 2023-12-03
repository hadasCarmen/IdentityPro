// cart.js

$(document).ready(function () {
    // Function to handle click on "view cart" link
    function onViewCartClick() {

        // Retrieve the orderId from local storage
        var orderId = localStorage.getItem('orderId');
        console.log('123!#' + orderId);

        // Parse orderId as an integer (or default to -1 if it's not a valid integer)
        var myId = parseInt(orderId) || -1;

        // Construct the URL based on myId
        var url = '/Default/Cart?orderId=' + myId; // Correct the controller name here

        // Redirect to the URL
        window.location.href = url;
    }

    // Attach click event handler to the "view cart" link
    $('#view-cart-link').on('click', onViewCartClick);


    // Function to handle click on "view cart" link
    function onViewCheckoutClick() {

        // Retrieve the orderId from local storage
        var orderId = localStorage.getItem('orderId');
        console.log('123!#' + orderId);

        // Parse orderId as an integer (or default to -1 if it's not a valid integer)
        var myId = parseInt(orderId) || -1;

        // Construct the URL based on myId
        var url = '/Default/Checkout?orderId=' + myId; // Correct the controller name here

        // Redirect to the URL
        window.location.href = url;
    }


    // Attach click event handler to the "view cart" link
    $('#view-checkout-link').on('click', onViewCheckoutClick);


    // Retrieve the orderId from local storage
	let orderId = localStorage.getItem('orderId') || -1;
	orderId = parseInt(orderId) || -1

    

    fetch('/Orders/getOrderData?orderId=' + orderId)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            console.log(data);

            if (data.products) {
                var totalAmount = data.products.reduce(function (accumulator, product) {
                    return accumulator + product.amount;
                }, 0);

                // Update the HTML with the total amount
                $('#product-count').text(totalAmount);

                // Select the <ul> element where you want to append the product items
                var productList = $('#product-list');
                productList.empty();

                // Loop through each product and generate HTML for it
                data.products.forEach(function (product) {
                    var listItem = $('<li>');

                    listItem.html(`
                    <div class="cart_section">
                        <div class="cart_img">
                            <a href="#"><img src="${product.imagePath}" alt=""></a>
                        </div>
                        <div class="cart_detail">
                            <h4><a href="cart.html">${product.name}</a></h4>
                            <h5>$ ${product.price}</h5>
                        </div>
                        <a class="cart_delete"></a>
                    </div>
                `);

                    productList.append(listItem);
                });
            }
        })
        .catch(error => {
            console.error('Fetch error:', error);
        });

       

});