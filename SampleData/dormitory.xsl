<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="html" encoding="utf-8" indent="yes"/>

  <xsl:template match="dormitory">
    <html>
      <head>
        <meta charset="utf-8"/>
        <title>Гуртожиток</title>
        <style>
          body { font-family: Segoe UI, Arial, sans-serif; background:#f5f6fb; margin:30px; color:#1f2937; }
          h1 { color:#4f46e5; }
          table { border-collapse:collapse; width:100%; background:white; box-shadow:0 2px 8px rgba(0,0,0,.08); }
          th { background:#4f46e5; color:white; text-align:left; padding:10px; }
          td { padding:10px; border-bottom:1px solid #e5e7eb; vertical-align:top; }
          tr:hover td { background:#eef2ff; }
          .now { color:#059669; font-weight:600; }
        </style>
      </head>
      <body>
        <h1>Мешканці гуртожитку</h1>
        <p>Усього студентів: <b><xsl:value-of select="count(student)"/></b></p>
        <table>
          <tr>
            <th>№</th><th>П.І.П.</th><th>Факультет</th><th>Кафедра</th><th>Курс</th><th>Телефон</th><th>Проживання</th>
          </tr>
          <xsl:apply-templates select="student">
            <xsl:sort select="@FullName"/>
          </xsl:apply-templates>
        </table>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="student">
    <tr>
      <td><xsl:value-of select="position()"/></td>
      <td><b><xsl:value-of select="@FullName"/></b></td>
      <td><xsl:value-of select="@Faculty"/></td>
      <td><xsl:value-of select="@Department"/></td>
      <td><xsl:value-of select="@Course"/></td>
      <td>
        <xsl:if test="@Phone"><xsl:value-of select="@Phone"/></xsl:if>
        <xsl:if test="not(@Phone)">—</xsl:if>
      </td>
      <td>
        <xsl:for-each select="residence">
          <div>
            <xsl:value-of select="@Building"/>, кімн. <xsl:value-of select="@Room"/>
            (<xsl:value-of select="@StartDate"/> –
            <xsl:choose>
              <xsl:when test="@EndDate"><xsl:value-of select="@EndDate"/></xsl:when>
              <xsl:otherwise><span class="now">дотепер</span></xsl:otherwise>
            </xsl:choose>)
          </div>
        </xsl:for-each>
      </td>
    </tr>
  </xsl:template>

</xsl:stylesheet>
