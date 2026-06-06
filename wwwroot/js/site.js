// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/*document.getElementById("submitTlacitkoSmlouvaCRT").addEventListener("click", async function () {

    const checkboxes = document.querySelectorAll('input[type="checkbox"]');
    const checkedOne = Array.from(checkboxes).some(x => x.checked);
    let checkedCheckboxy = Array.from(checkboxes).filter(x => x.checked).map(x => x.value);

    if (!checkedOne) {
        return alert("ani jeden spravce neni vybrany")
    }

    /*let dSID = document.getElementById("datumPlatnostiID").value
    let datumSplatnostiFormat = `${dSID.getDate()}. ${dSID.getMonth()}. ${dSID.getFullYear()}`

    let dUID = document.getElementById("datumUkonceniID").value
    let datumUkonceniFormat = `${dUID.getDate()}. ${dUID.getMonth()}. ${dUID.getFullYear()}`

    const smlouvaData = {
        instituce: document.getElementById("instituceID").value,
        klient: document.getElementById("klientID").value,
        spravce: checkedCheckboxy,
        datumPlatnosti: document.getElementById("datumPlatnostiID").value,
        datumUkonceni: document.getElementById("datumUkonceniID").value,
    }

    const response = await fetch('/Smlouva/VytvoritSmlouvu', {
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(smlouvaData)
    })

    if (response.ok) {
        const result = await response.json();
        alert(result.message);
    }
    else {
        alert("Neco se nepovedlo")
    }
})*/
