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


