<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    
  <xsl:output method="xml" indent="yes"/>


  <xsl:param name="dbServer" />
  <xsl:param name="ApplicationName"/>
  
  
    <xsl:template match="@* | node()">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()"/>
        </xsl:copy>
    </xsl:template>



   <!-- This is the template that performs the connection string transformation-->
  <xsl:template match="/configuration/connectionStrings">

    <connectionStrings>
      <add>
        <xsl:attribute name="name">ConnStr</xsl:attribute>
        <xsl:attribute name="connectionString">DSN=<xsl:value-of select="$ApplicationName"/>;Trusted_Connection=Yes;</xsl:attribute>
      </add>


      <add>
        <xsl:attribute name="name">fms8Dict</xsl:attribute>
        <xsl:attribute name="connectionString">Data Source=<xsl:value-of select="$dbServer"/>;Initial Catalog=<xsl:value-of select="$ApplicationName"/>;PERSIST SECURITY INFO=False;Integrated Security=True;</xsl:attribute>
        <xsl:attribute name="providerName">System.Data.SqlClient</xsl:attribute>
      </add>

      <add>
        <xsl:attribute name="name">fms8Sec</xsl:attribute>
        <xsl:attribute name="connectionString">Data Source=<xsl:value-of select="$dbServer"/>;Initial Catalog=<xsl:value-of select="$ApplicationName"/>_ASPNET;PERSIST SECURITY INFO=False;Integrated Security=True;</xsl:attribute>
        <xsl:attribute name="providerName">System.Data.SqlClient</xsl:attribute>
      </add>
    </connectionStrings>

  </xsl:template>

</xsl:stylesheet>
