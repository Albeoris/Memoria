# check we're elivated
if (-Not ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] 'Administrator')) {
    if ([int](Get-CimInstance -Class Win32_OperatingSystem | Select-Object -ExpandProperty BuildNumber) -ge 6000) {
        $CommandLine = "-File `"" + $MyInvocation.MyCommand.Path + "`" " + $MyInvocation.UnboundArguments
        Start-Process -FilePath PowerShell.exe -Verb Runas -ArgumentList $CommandLine
        Exit
    }
}

Set-Location $PSScriptRoot;

# make sure windows feature is installed
$net3State = Get-WindowsCapability -Online -Name NetFx3

if($net3State.State -ne "Installed"){
    Add-WindowsCapability -Online -Name NetFx3
}

$skdInstallPath = "C:\Program Files (x86)\Windows Kits\8.1\bin\x64"
$pathState = Test-Path -Path $skdInstallPath
if(!$pathState){
    Invoke-WebRequest https://go.microsoft.com/fwlink/p/?LinkId=323507 -OutFile sdksetup.exe
    Start-Process sdksetup.exe -Wait
}


# find the game install location
$appId = "377840";
$gameInstalled = Test-Path -Path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App $($appId)"
if(!$gameInstalled){
    Write-Error "Final Fantasy IX is not installed via Steam we don't support piracy"
    exit
}
$ff9InstallLoc = Get-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App $($appId)"

# copy files
$filesPath = "$($ff9InstallLoc.InstallLocation)\x64\FF9_Data\Managed\"
$dest = Resolve-Path "./References/"

get-childitem $filesPath -recurse | Where-Object {$_.extension -eq ".dll"} | ForEach-Object {
    Write-Output "Copying $($_.Name)"
    Copy-Item -Path $_.FullName -Destination $dest
}

Write-Output "Project has been setup you can now open the solution file."
read-host "Press ENTER to exit"
# SIG # Begin signature block
# MIIRYQYJKoZIhvcNAQcCoIIRUjCCEU4CAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCAE/XfdPtNWSBJh
# 2QIm89A8mstsNNXwjPaCvmiyy6HPraCCDaQwgga5MIIEoaADAgECAhEAmaOACiZV
# O2Wr3G6EprPqOTANBgkqhkiG9w0BAQwFADCBgDELMAkGA1UEBhMCUEwxIjAgBgNV
# BAoTGVVuaXpldG8gVGVjaG5vbG9naWVzIFMuQS4xJzAlBgNVBAsTHkNlcnR1bSBD
# ZXJ0aWZpY2F0aW9uIEF1dGhvcml0eTEkMCIGA1UEAxMbQ2VydHVtIFRydXN0ZWQg
# TmV0d29yayBDQSAyMB4XDTIxMDUxOTA1MzIxOFoXDTM2MDUxODA1MzIxOFowVjEL
# MAkGA1UEBhMCUEwxITAfBgNVBAoTGEFzc2VjbyBEYXRhIFN5c3RlbXMgUy5BLjEk
# MCIGA1UEAxMbQ2VydHVtIENvZGUgU2lnbmluZyAyMDIxIENBMIICIjANBgkqhkiG
# 9w0BAQEFAAOCAg8AMIICCgKCAgEAnSPPBDAjO8FGLOczcz5jXXp1ur5cTbq96y34
# vuTmflN4mSAfgLKTvggv24/rWiVGzGxT9YEASVMw1Aj8ewTS4IndU8s7VS5+djSo
# McbvIKck6+hI1shsylP4JyLvmxwLHtSworV9wmjhNd627h27a8RdrT1PH9ud0IF+
# njvMk2xqbNTIPsnWtw3E7DmDoUmDQiYi/ucJ42fcHqBkbbxYDB7SYOouu9Tj1yHI
# ohzuC8KNqfcYf7Z4/iZgkBJ+UFNDcc6zokZ2uJIxWgPWXMEmhu1gMXgv8aGUsRda
# CtVD2bSlbfsq7BiqljjaCun+RJgTgFRCtsuAEw0pG9+FA+yQN9n/kZtMLK+Wo837
# Q4QOZgYqVWQ4x6cM7/G0yswg1ElLlJj6NYKLw9EcBXE7TF3HybZtYvj9lDV2nT8m
# FSkcSkAExzd4prHwYjUXTeZIlVXqj+eaYqoMTpMrfh5MCAOIG5knN4Q/JHuurfTI
# 5XDYO962WZayx7ACFf5ydJpoEowSP07YaBiQ8nXpDkNrUA9g7qf/rCkKbWpQ5bou
# fUnq1UiYPIAHlezf4muJqxqIns/kqld6JVX8cixbd6PzkDpwZo4SlADaCi2JSplK
# ShBSND36E/ENVv8urPS0yOnpG4tIoBGxVCARPCg1BnyMJ4rBJAcOSnAWd18Jx5n8
# 58JSqPECAwEAAaOCAVUwggFRMA8GA1UdEwEB/wQFMAMBAf8wHQYDVR0OBBYEFN10
# XUwA23ufoHTKsW73PMAywHDNMB8GA1UdIwQYMBaAFLahVDkCw6A/joq8+tT4HKbR
# Og79MA4GA1UdDwEB/wQEAwIBBjATBgNVHSUEDDAKBggrBgEFBQcDAzAwBgNVHR8E
# KTAnMCWgI6Ahhh9odHRwOi8vY3JsLmNlcnR1bS5wbC9jdG5jYTIuY3JsMGwGCCsG
# AQUFBwEBBGAwXjAoBggrBgEFBQcwAYYcaHR0cDovL3N1YmNhLm9jc3AtY2VydHVt
# LmNvbTAyBggrBgEFBQcwAoYmaHR0cDovL3JlcG9zaXRvcnkuY2VydHVtLnBsL2N0
# bmNhMi5jZXIwOQYDVR0gBDIwMDAuBgRVHSAAMCYwJAYIKwYBBQUHAgEWGGh0dHA6
# Ly93d3cuY2VydHVtLnBsL0NQUzANBgkqhkiG9w0BAQwFAAOCAgEAdYhYD+WPUCia
# U58Q7EP89DttyZqGYn2XRDhJkL6P+/T0IPZyxfxiXumYlARMgwRzLRUStJl490L9
# 4C9LGF3vjzzH8Jq3iR74BRlkO18J3zIdmCKQa5LyZ48IfICJTZVJeChDUyuQy6rG
# DxLUUAsO0eqeLNhLVsgw6/zOfImNlARKn1FP7o0fTbj8ipNGxHBIutiRsWrhWM2f
# 8pXdd3x2mbJCKKtl2s42g9KUJHEIiLni9ByoqIUul4GblLQigO0ugh7bWRLDm0Cd
# Y9rNLqyA3ahe8WlxVWkxyrQLjH8ItI17RdySaYayX3PhRSC4Am1/7mATwZWwSD+B
# 7eMcZNhpn8zJ+6MTyE6YoEBSRVrs0zFFIHUR08Wk0ikSf+lIe5Iv6RY3/bFAEloM
# U+vUBfSouCReZwSLo8WdrDlPXtR0gicDnytO7eZ5827NS2x7gCBibESYkOh1/w1t
# VxTpV2Na3PR7nxYVlPu1JPoRZCbH86gc96UTvuWiOruWmyOEMLOGGniR+x+zPF/2
# DaGgK2W1eEJfo2qyrBNPvF7wuAyQfiFXLwvWHamoYtPZo0LHuH8X3n9C+xN4YaNj
# t2ywzOr+tKyEVAotnyU9vyEVOaIYMk3IeBrmFnn0gbKeTTyYeEEUz/Qwt4HOUBCr
# W602NCmvO1nm+/80nLy5r0AZvCQxaQ4wggbjMIIEy6ADAgECAhAq7DWNNirGIt8P
# XGPNNOODMA0GCSqGSIb3DQEBCwUAMFYxCzAJBgNVBAYTAlBMMSEwHwYDVQQKExhB
# c3NlY28gRGF0YSBTeXN0ZW1zIFMuQS4xJDAiBgNVBAMTG0NlcnR1bSBDb2RlIFNp
# Z25pbmcgMjAyMSBDQTAeFw0yNDAyMTUxMzU2NDVaFw0yNTAyMTQxMzU2NDRaMIGI
# MQswCQYDVQQGEwJHQjEXMBUGA1UECAwOV2VzdCBZb3Jrc2hpcmUxETAPBgNVBAcM
# CFdldGhlcmJ5MR4wHAYDVQQKDBVPcGVuIFNvdXJjZSBEZXZlbG9wZXIxLTArBgNV
# BAMMJE9wZW4gU291cmNlIERldmVsb3BlciwgTWFydGluIEJhcmtlcjCCAiIwDQYJ
# KoZIhvcNAQEBBQADggIPADCCAgoCggIBALE13ASvl1Csl2kAVsTmEJmzhRmPoRGd
# +O6rhtCfY6gRsQt2pSIIpMiE57a8O2SxxbfweLwpkVZKKLsrgGPqvbD//b9pibnv
# IK3Bp93cChdrXZeKz+SYq7aaVeLAgjD7iBQcANqK0nO4dk95W6kPlWyWsk7VTOMO
# YXEs6q4UZFQCNYgxuquJX4JZe09UEim0zZn/qKDOB3qWhcx0K5e4QBE/4xxvtA5E
# jGhXsNMH/LZeeqWlFOyTzqPxXaOz3WcYXdUd1CC2Ss5GXTVAer20ddST9J9mIVDA
# 2XVPpoSQ3MQ1a+/j07ZM8iHpqpek2oZYAufPIq0Uwn0RCmKwJ7mEpa5DdeN2aUF2
# afy0sN9M7jMG3olpXzpERpx6k+Pe+RdIvjZ+G/f+n9PSHJlf9E6tfqwuFLLVk1mn
# UAehmfOb3gwiHjIaCu+imSYxbigaVwZ7m/aEkq82xZTsQVcQSJZ/cqlBsHyOv5Lp
# sM8iL1xkPm8qntCKV/HylLfbvYSEfp1BgMpgONjF2KoNrobiwzf7vCSafWQH4oXN
# wJ4ZRRH7F5DKYyGF1wQVpwp9PJUNh4jQvhmp1Ir/MYDM42eQQnsR15VGBFGkAwd1
# Ck6uaEYkdkN6u6FEo4fO7prGqnEriiZez+o8BQ/cygxeZAm8kZ0Iv3cZXqQd9lQu
# Fz6PIVH/iXVJAgMBAAGjggF4MIIBdDAMBgNVHRMBAf8EAjAAMD0GA1UdHwQ2MDQw
# MqAwoC6GLGh0dHA6Ly9jY3NjYTIwMjEuY3JsLmNlcnR1bS5wbC9jY3NjYTIwMjEu
# Y3JsMHMGCCsGAQUFBwEBBGcwZTAsBggrBgEFBQcwAYYgaHR0cDovL2Njc2NhMjAy
# MS5vY3NwLWNlcnR1bS5jb20wNQYIKwYBBQUHMAKGKWh0dHA6Ly9yZXBvc2l0b3J5
# LmNlcnR1bS5wbC9jY3NjYTIwMjEuY2VyMB8GA1UdIwQYMBaAFN10XUwA23ufoHTK
# sW73PMAywHDNMB0GA1UdDgQWBBTgRJP3FEE16hav/ZflJzL2rqa5LDBLBgNVHSAE
# RDBCMAgGBmeBDAEEATA2BgsqhGgBhvZ3AgUBBDAnMCUGCCsGAQUFBwIBFhlodHRw
# czovL3d3dy5jZXJ0dW0ucGwvQ1BTMBMGA1UdJQQMMAoGCCsGAQUFBwMDMA4GA1Ud
# DwEB/wQEAwIHgDANBgkqhkiG9w0BAQsFAAOCAgEAXNKnb+F6HqxAilPsRjC5L2jO
# 8vHi4PDLVRQM5wWI+FwhLO47RQjmPayr5FF5ZjK9w/Rls5AWNxhMPJhSv2MZCIpz
# ztveO2rYD9taRURvbPbY3/srHyBpeq2vc+quozGQ6reYkdoLYIsqPy9u3seZM2e1
# E98SHU5OoxDqJZ2X9i36w2isq/0GP0jE8dLJUgCneDn1sPElCntehxY9Ivj1wcpM
# F2ZQvJAeDBeENX7KWuFa755UI1lf2GhSXrmcYpEnM+j4v4ViYuspgzcE6nZvupTB
# IijidTTYIEfd2cArP1arCQFDeBwp47BWRBAPFOqXXxKVyHX1c4EUpcA0CBNb13pV
# xQKmpQ1qDIzrub7PNgk3U2QDD4FzeG83aa6jKtNXfd2zaE5+LmKsVs3X7qRQEMoq
# mUxUzK8jKp2VzhUFE4s216EHDLWxY2H6pIkP44h5mC7wlVy5sdxPf/eCEqGYKLL9
# SHeYwdIdw6d8pfgpjD0spMFPQvct1KH5TC6aLCTCte4NiQTq5uTocXokrWMOMfWL
# cXYsid0+Ko+rwSV7v2sYunXyAIO3Q1Qpdg0FMlMCmfMrUsDZyz7ZPG092VfJ2to2
# DACjoh7yJlRcrysw/gyw3He8mW8VE25QAQWLCtyjmHA7b59l9jUCHTkVpoVHYWBs
# w5kHDP7VjdlvYBUBMZ4xggMTMIIDDwIBATBqMFYxCzAJBgNVBAYTAlBMMSEwHwYD
# VQQKExhBc3NlY28gRGF0YSBTeXN0ZW1zIFMuQS4xJDAiBgNVBAMTG0NlcnR1bSBD
# b2RlIFNpZ25pbmcgMjAyMSBDQQIQKuw1jTYqxiLfD1xjzTTjgzANBglghkgBZQME
# AgEFAKB8MBAGCisGAQQBgjcCAQwxAjAAMBkGCSqGSIb3DQEJAzEMBgorBgEEAYI3
# AgEEMBwGCisGAQQBgjcCAQsxDjAMBgorBgEEAYI3AgEVMC8GCSqGSIb3DQEJBDEi
# BCDgOtwC/s64NAW0LHbWGdzo0SSeJbmZ2wJ9D+/jkTtl6DANBgkqhkiG9w0BAQEF
# AASCAgB9JdbKbUMm8MheOYIdn1lom7TSRoabFSn9rypfFlfqTnK7Hj9/p2umvUl8
# x6XkZPNfpoJwq/o48BGXPLHwIn9uFpoS0Hr8V7Iz1KslN9jip/tm9y+hWiYjgpvv
# FJv2vYFV7XeEqfCsT/Xb0CnmtRKr/uA6Wn5p58V7Ku8vjaCkGo2qUL4YijLx7BcT
# mz6LC45ofL9TntYGuuhU5vr8DfoHaMD8fxY2lZU2IsCgbbmpyJyDvw1stPpgluvB
# cVpx3zLX3Kivi3bQmfwdlvteA+8tVkoIk460pIeg5glvuoRZMI/Yo5fXNh0Q1npl
# 6ftSKPQfrL3U5KhumoRJadMlmMDGltfCImxvaRWO0cRc/Wpk3r7remDrPtWo4Zo+
# 2C0c+xy2/q5oGxyUY6eNLE1TS1xFHGBzpTrGNxPri2tcNz2d4Lsp11ClGu53n7XP
# 6IM7T9unHebEHvtYPymYT44ebq8YY0h44c31u2m8KPQLLkxH0GVpBuQ0DPMrnuMc
# QBG+ZCi23gl1QO3m6gdJftN7nweU/E57FcHxN2rS8UhwI0Oq1k5iL764grC95E4k
# p0TACuXvUD5K7jbXALVCRG4AJeZa8ns170pEjoeEhBS5/GQliyWFx4XqE6h+/Csw
# TJqsziSOi6Rij3lhg+xrZYyRyw5ia7z2dcDDwezG66VQ3AlvdA==
# SIG # End signature block
