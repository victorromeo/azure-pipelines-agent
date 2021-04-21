function Parse-Version  {
    <#
        .SYNOPSIS
            Parses version from provided. Allows incomplete versions like 16.0. Throws exception if there is more than 4 numbers divided by dot.

        .EXAMPLE
            Parse-Version -Version "1.3.5" 
            
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Version)

    if ($Version.IndexOf(".") -lt 0) {
        return [System.Version]::Parse("$($Version).0")
    }

    return [System.Version]::Parse($Version)
}