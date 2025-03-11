// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function removeRememberMe() {
    document.cookie = "rememberMe=; path=/; max-age=0; secure; SameSite=Strict";
}
function setRememberMe(token) {
    document.cookie = `rememberMe=${token}; path=/; max-age=${60 * 60 * 24 * 30}; secure; SameSite=Strict`;
}

function getRememberMe() {
    const cookies = document.cookie.split("; ");
    for (let cookie of cookies) {
        const [name, value] = cookie.split("=");
        if (name === "rememberMe") {
            return value;
        }
    }
    return null;
}

function handleSignOut() {
    removeRememberMe();
    window.location.href = "/auth/signout";
}

function removeRememberMe() {
    document.cookie = "rememberMe=; path=/; max-age=0; secure; SameSite=Strict";
}

// cart function 
function addCart(bookId) {
    $.ajax({
        url: '/Orders/AddTocart',
        type: 'POST',
        data: { bookId: bookId },
        success: function (result) {
            // Update cart count in the navigation
            updateCartCount(result.cartCount);
            if (result.cartCount === 0) {
                location.reload();
            }
        }
    });

}

function removeFromCart(bookId) {
    $.ajax({
        url: '/Orders/RemoveFromCart',
        type: 'POST',
        data: { bookId: bookId },
        success: function (result) {
            if (result.success) {
                $('#book-' + bookId).remove();

                // Update cart count in the navigation
                updateCartCount(result.cartCount);

                // If cart is empty, refresh the page to show empty cart message
                if (result.cartCount === 0) {
                    location.reload();
                }
            }
        }
    });
}
function updateCartCount(count) {
    // Assuming you have a cart count display in your layout
    $('#cart-count').text(count);
}



