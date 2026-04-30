FROM mcr.microsoft.com/dotnet/framework/wcf:4.7.2
EXPOSE 80
ARG source
WORKDIR /inetpub/wwwroot
COPY ${source:-obj/Docker/publish} .

# Security: create non-root user and grant read access to web content (CWE-250)
RUN powershell -Command "New-LocalUser -Name 'appuser' -NoPassword -AccountNeverExpires; \
    Add-LocalGroupMember -Group 'IIS_IUSRS' -Member 'appuser'"
RUN icacls "C:\inetpub\wwwroot" /grant "appuser:(OI)(CI)R" /T
USER appuser
