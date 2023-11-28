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

    fetch('/Orders/getOrderData?orderId=' + orderId).then(response => {
        // 3. Handle the response
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        // Parse the response as JSON
        return response.json();
    })
        .then(data => {
            // Use the data here
            console.log(data);

            // Now you can access the products using data.Products
            if (data.products) {
                var totalAmount = data.products.reduce(function (accumulator, product) {
                    return accumulator + product.amount;
                }, 0);

                // Update the HTML with the total amount
                $('#product-count').text(totalAmount);
                var products = data.products; // Replace 'data.Products' with your actual data source

                // Select the <ul> element where you want to append the product items
                var productList = $('#product-list');
                // Loop through each product and generate HTML for it
                products.forEach(function (product) {
                    // Create a new <li> element
                    var listItem = $('<li>');

                    // Generate the HTML for the product item and append it to the <li> element
                    listItem.html(`
            <div class="cart_section">
                <div class="cart_img">
                    <a href="#"><img src="/images/shop/${product.imagePath}" alt=""></a>
                </div>
                <div class="cart_detail">
                    <h4><a href="cart.html">${product.name}</a></h4>
                    <h5>$ ${product.price}</h5>
                </div>
                <a class="cart_delete"></a>
            </div>
        `);

                    // Append the <li> element to the <ul> (productList)
                    productList.append(listItem);
                });
            }
           
        })
        .catch(error => {
            console.error('Fetch error:', error);
        });

       

});