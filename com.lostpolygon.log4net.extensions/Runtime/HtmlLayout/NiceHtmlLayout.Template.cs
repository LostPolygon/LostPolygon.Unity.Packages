namespace LostPolygon.Log4netExtensions{
    public partial class NiceHtmlLayout {
        public NiceHtmlLayout(string pattern) : base(pattern) {
            //language=css
            CustomCss = @"
        .item-logger {
            font-weight: 600;
        }

        .log-message-stacktrace {
            display: none;
            margin-top: 0.5em;
            line-height: 1.3;
        }
";

            //language=javascript
            CustomJavascriptBeforeLoad = @"
checkIfMustUseExpandCollapseFunction = function(row, index) {
    var result = row.cellsText[index].length > maxTextLengthBeforeCollapse
    if (!result)
        return false;

    var logException = row.cells[index].querySelector('.log-message-stacktrace')
    if (logException != null) {
        result = (row.cellsText[index].length - logException.innerText.length) > maxTextLengthBeforeCollapse
    }
    return result;
}
";

            //language=javascript
            CustomJavascriptAfterLoad = @"
        var showStackTraces = false
        var logStackTraces = $("".log-message-stacktrace"")

        var exceptionSwitchCheckbox = document.createElement('input')
        exceptionSwitchCheckbox.type = ""checkbox"";
        exceptionSwitchCheckbox.name = ""name"";
        exceptionSwitchCheckbox.id = ""exception-switch"";

        var exceptionSwitchCheckboxLabel = document.createElement('label')
        exceptionSwitchCheckboxLabel.htmlFor = exceptionSwitchCheckbox.id
        exceptionSwitchCheckboxLabel.innerHTML = ""&nbsp;Show stack traces"";

        filterInput[0].parentNode.insertBefore(exceptionSwitchCheckbox, filterInput[0].nextSibling)
        filterInput[0].parentNode.insertBefore(exceptionSwitchCheckboxLabel, exceptionSwitchCheckbox.nextSibling)

        function setShowLogStackTraces(show) {
            exceptionSwitchCheckbox.checked = show
            if (show) {
                logStackTraces.show()
            } else {
                logStackTraces.hide()
            }
        }

        exceptionSwitchCheckbox.onchange = function() {
            showStackTraces = !showStackTraces
            logTable.hide()
            setShowLogStackTraces(showStackTraces)
            logTable.show()
        }

        setShowLogStackTraces(showStackTraces)
";
        }
    }
}
