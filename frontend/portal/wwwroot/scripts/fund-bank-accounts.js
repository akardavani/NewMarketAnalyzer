/*FundBankAccounts*/
// copy data to clipboard when user clicks on a button
function copyToClipboard(button) {
    const row = button.closest('tr'); //to find the parent row
    const dataToCopy = row.querySelector('.data-to-copy');
    const icon = button.querySelector('svg');
    const tooltip = button.querySelector('.tooltip-text');

    const textToCopy = dataToCopy.textContent.trim();

    navigator.clipboard.writeText(textToCopy)
        //Update the icon and its tooltip for successful copy
        .then(() => {
            icon.classList.remove('bi-copy');
            icon.classList.add('bi-check');
            icon.style.color = '#E8885E';
            tooltip.textContent = "کپی شد";

            //Reset the icon and tooltip after 2 seconds of successful copy
            setTimeout(function () {
                icon.classList.remove('bi-check');
                icon.classList.add('bi-copy');
                icon.style.color = 'currentColor';
            }, 2000);
        })
}
