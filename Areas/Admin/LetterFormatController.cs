using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Tangehrine.WebLayer.Utility;
using Tangehrine.ServiceLayer.Dtos.Admin;
using Tangehrine.ServiceLayer.Interface.Admin.Master;
using Microsoft.AspNetCore.Http;
using System;
using Tangehrine.ServiceLayer;
using SelectPdf;

namespace Tangehrine.WebLayer.Areas.Admin
{
    [Authorize, Area("Admin")]    
    public class LetterFormatController : BaseController<LetterFormatController>
    {
        #region Fields

        private readonly ILetterFormatService _letterFormatService;

        #endregion

        #region Ctor
        public LetterFormatController(ILetterFormatService letterFormatService)
        {
            _letterFormatService = letterFormatService;
        }
        #endregion
        #region Method
        public IActionResult Index()
        {
            //return View();
            return View("LetterFormatList");
        }
        [HttpGet]
        public async Task<IActionResult> LetterFormatList()
        {
            var letterFormatData = await _letterFormatService.GetLetterFormatList();
            return Json(letterFormatData);
        }
        [HttpGet]
        public async Task<IActionResult> AddEditLetterFormat(long id)
        {
            LetterFormatDto letterFormatDto = new LetterFormatDto();
            if (id > 0)
            {
                letterFormatDto = await _letterFormatService.GetLetterFormatById(id);

                DateTime date = Convert.ToDateTime(letterFormatDto.Date);

                var headerHtml = "<!DOCTYPE html> <html dir='ltr' lang='en-US'> <head> <title>Letter Format | TangEHRine</title>  <link type='image/x-icon' rel='shortcut icon' href='../assets/images/favicon.png' />         <!-- Required meta tags -->         <meta charset='UTF-8' />         <meta name='HandheldFriendly' content='true' />         <meta name='viewport' content='width=device-width, initial-scale=1.0' /> <link rel='preconnect' href='https://fonts.googleapis.com' />         <link rel='preconnect' href='https://fonts.gstatic.com' crossorigin />         <link href='https://fonts.googleapis.com/css2?family=Poppins:wght@200;300;400;500;600;700;800;900&display=swap' rel='stylesheet' />          <style type='text/css'>             /*********************  Default-CSS  *********************/              input[type='file']::-webkit-file-upload-button {                 cursor: pointer;             }              input[type='file']::-moz-file-upload-button {cursor: pointer;}  input[type='file']::-ms-file-upload-button {                 cursor: pointer;             }              input[type='file']::-o-file-upload-button {                 cursor: pointer;             }              input[type='file'],             a[href],             input[type='submit'],             input[type='button'],             input[type='image'],             label[for],select,button,.pointer {cursor: pointer;}::-moz-focus-inner { border: 0px solid transparent;             }              ::-webkit-focus-inner {border: 0px solid transparent;}   *::-moz-selection {                 color: #fff;                 background: #000;             }              *::-webkit-selection {                 color: #fff;                 background: #000;             }              *::-webkit-input-placeholder {                 color: #333333;                 opacity: 1;             }              *:-moz-placeholder {                 color: #333333;                 opacity: 1;             }              *::-moz-placeholder {                 color: #333333;                 opacity: 1;             }              *:-ms-input-placeholder {                 color: #333333;                 opacity: 1;             }             html {                 font-size: 18px;             }             html body {                 font-family: 'Poppins', sans-serif;                 margin: 0;                 line-height: 1.5;                 font-size: 1rem;             }              a,             div a:hover,             div a:active,             div a:focus,             button {                 text-decoration: none;                 -webkit-transition: all 0.5s ease 0s;                 -moz-transition: all 0.5s ease 0s;                 -ms-transition: all 0.5s ease 0s;                 -o-transition: all 0.5s ease 0s;                 transition: all 0.5s ease 0s;             }              a,             span,             div a:hover,             div a:active,             button {                 text-decoration: none;             }              *::after,             *::before,             * {                 -webkit-box-sizing: border-box;                 -moz-box-sizing: border-box;                 -ms-box-sizing: border-box;                 -o-box-sizing: border-box;                 box-sizing: border-box;             }              .no-list li,             .no-list ul,             .no-list ol,             footer li,             footer ul,             footer ol,             header li,             header ul,             header ol {                 list-style: inside none none;             }              .no-list ul,             .no-list ol,             footer ul,             footer ol,             header ul,             header ol {                 margin: 0;                 padding: 0;             }              a {                 outline: none;                 color: #555;             }              a:hover {                 color: #000;             }              body .clearfix,             body .clear {                 clear: both;                 line-height: 100%;             }              body .clearfix {                 height: auto;             }              * {                 outline: none !important;             }              table {                 border-collapse: collapse;                 border-spacing: 0;             }              ul:after,             li:after,             .clr:after,             .clearfix:after,             .container:after,             .grve-container:after {                 clear: both;                 display: block;                 content: '';             }              div input,             div select,             div textarea,             div button {                 font-family: 'Poppins', sans-serif;             }              body h1,             body h2,             body h3,             body h4,             body h5,             body h6 {                 font-family: 'Poppins', sans-serif;                 line-height: 1.5;                 color: #333;                 font-weight: 600;                 margin: 0 0 15px;             }              body h1:last-child,             body h2:last-child,             body h3:last-child,             body h4:last-child,             body h5:last-child,             body h6:last-child {                 margin-bottom: 0;             }              .h2,             h2 {                 font-size: 1.7rem;             }              div select {                 overflow: hidden;                 text-overflow: ellipsis;                 white-space: nowrap;             }              img,             svg {                 margin: 0 auto;                 max-width: 100%;                 max-height: 100%;                 width: auto;                 height: auto;                 vertical-align: top;             }              body p {                 margin: 0 0 25px;                 padding: 0;             }              body p:empty {                 margin: 0;                 line-height: 0;             }              body p:last-child {                 margin-bottom: 0;}  p strong { font-weight: 600; }   strong { font-weight: 600 } .a-left {                 text-align: left;  } .a-right {  text-align: right; } .a-center {  text-align: center; } .hidden { display: none !important;             }              body .container .container { width: 100%; max-width: 100%; }          /*********************  Default-CSS close  *********************/                    .back-btn {                 width: 48px;fill:#fff;margin-right: auto; } .back-btn:hover, .action-btns:hover { opacity: 0.5;}.action-btns {  background: transparent;  border: none;height: 30px;                 fill: #fff;padding: 0 25px;}   .main-padding {padding: 5% 10%;}  footer img {width: 100%;vertical-align: top;}  #headerLetter {                 background: rgba(0, 0, 0, 0.5);                 padding: 30px;                 display: flex;                 align-items: center;             }             .back-btn {                 width: 48px;                 fill: #fff;margin-right: auto;} .back-btn:hover,.action-btns:hover { opacity: 0.5;} .action-btns { background: transparent;  border: none;   height: 30px;fill:#fff;padding: 0 25px; } .main-padding {padding: 5% 10%;} footer img {width:100%;vertical-align: top;}</style></head><body>";

                var headerMiddleHtml = "<header id='headerLetter'>  <a href='#' onclick='HideLetterFormat()' class='back-btn'>  <svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px' viewBox='0 0 48 30' style='enable-background: new 0 0 48 30;' xml:space='preserve'>  <path d='M13.3,29.4l-12.7-13c-0.8-0.8-0.8-2.1,0-2.9l12.7-13c0.8-0.8,2-0.8,2.8,0c0.8,0.8,0.8,2.1,0,2.9L6.8,13H46 c1.1,0,2,0.9,2,2s-0.9,2-2,2H6.8l9.3,9.5c0.8,0.8,0.8,2.1,0,2.9C15.4,30.2,14.1,30.2,13.3,29.4z' />                 </svg> </a> <button class='action-btns'> <svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px' viewBox='0 0 33 29' style='enable-background: new 0 0 33 29;' xml:space='preserve'> <path d='M28,8.1H4.9C2.2,8.1,0,10.2,0,12.9v9.7h6.6V29h19.8v-6.4H33v-9.7C33,10.2,30.8,8.1,28,8.1z M23.1,25.8H9.9v-8.1 h13.2V25.8z M28,14.5c-0.9,0-1.6-0.7-1.6-1.6s0.7-1.6,1.6-1.6c0.9,0,1.7,0.7,1.7,1.6S29,14.5,28,14.5z M26.4,0H6.6v6.4h19.8V0z'/>  </svg>  </button>             <button class='action-btns'>                 <svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' x='0px' y='0px' viewBox='0 0 25 30' style='enable-background: new 0 0 25 30;' xml:space='preserve'>                     <path d='M25,10.6h-7.1V0H7.1v10.6H0l12.5,12.4L25,10.6z M0,26.5V30h25v-3.5H0z' /></svg></button> </header>";
                //middle body
                var middleHtml = "<div class='main-padding'> <table style='font-family: 'Poppins', sans-serif; font-weight: 400; width: 100%; border-collapse: collapse; border-spacing: 0; color: #333; line-height: 2;'><tbody><tr style='vertical-align: middle;'> <td><img src='../assets/images/logo.png' alt='logo' /></td><td style='text-align: right;'>  " +
                                "<div style='margin-bottom: 15px;'> <span style='vertical-align: middle; display: inline-block; margin-right: 10px;'> +" + letterFormatDto.Phone1 + "<br />+" + letterFormatDto.Phone2 + "</span><span style='vertical-align: middle; display: inline-block;'><img src='../assets/images/file-telephone.jpg' alt='file-telephone' /></span>" +
                                "</div><div style='margin-bottom: 15px;'><span style='vertical-align: middle; display: inline-block; margin-right: 10px;'>" + letterFormatDto.Website + "<br />" + letterFormatDto.Email + "</span> <span style='vertical-align: middle; display: inline-block;'><img src='../assets/images/file-mail.jpg' alt='file-mail' /></span>  </div>  <div> <span style='vertical-align: middle; display: inline-block; margin-right: 10px;'>" + letterFormatDto.Address + "</span>  <span style='vertical-align: middle; display: inline-block;'><img src='../assets/images/file-address.jpg' alt='file-address' /></span></div></td></tr><tr><td height='100' colspan='2'></td></tr><tr style='vertical-align: bottom;'><td><div><strong style='color: #f17d2e; font-weight: 600; vertical-align: top;'>" + letterFormatDto.ReciverFullName + "</strong></div><div><small style='vertical-align: top;'>" + letterFormatDto.ReciverPossition + "</small></div><div><strong style='color: #f17d2e; vertical-align: top;'>A : </strong>" + letterFormatDto.ReciverAddress + "</div><div><strong style='vertical-align: top;'>W : </strong>" + letterFormatDto.ReciverEmail + "," + letterFormatDto.ReciverWebsite + "</div><div><strong style='color: #f17d2e; vertical-align: top;'>P : </strong>" + letterFormatDto.ReciverPhone + "</div></td> <td style='text-align: right;'> Date," + date.ToString("dd") +" "+ date.ToString("MMMM") +", " + date.Year + "</td></tr><tr><td height='100' colspan='2'></td></tr><tr> <td colspan='2'>" + letterFormatDto.LetterFormatTitle + letterFormatDto.LetterFormatContent;

                var footerHtml = " </td></tr> <tr>  <td height='100' colspan='2'></td> </tr><tr><td colspan='2'> <div><img src='../assets/images/signature.png' alt='signature' style='vertical-align: top;' /></div><div><strong style='vertical-align: top;'>John Smeeth</strong></div> <div><small style='vertical-align: top;'>Manager</small></div>                         </td></tr> </tbody> </table></div><footer>             <img src='../assets/images/file-footer.png' alt='file-footer' />         </footer>     </body> </html>";

                letterFormatDto.HtmlString = headerHtml + headerMiddleHtml + middleHtml + footerHtml;
            }
            return Json(letterFormatDto);
        }
        [HttpGet]        
        public async Task<IActionResult> LetterFormatDownload(long id)
        {
            var response = new ServiceResponse();
            try
            {
                LetterFormatDto letterFormatDto = new LetterFormatDto();
                if (id > 0)
                {
                    letterFormatDto = await _letterFormatService.GetLetterFormatById(id);
                    DateTime date = Convert.ToDateTime(letterFormatDto.Date);

                    var url = $"{this.Request.Scheme}://{this.Request.Host}";
                    var headerHtmlForDownload = "<!DOCTYPE html> <html dir='ltr' lang='en-US'>     <head>         <title>Letter Format | TangEHRine</title>         <link type='image/x-icon' rel='shortcut icon' href='../assets/images/favicon.png' />         <!-- Required meta tags -->         <meta charset='UTF-8' />         <meta name='HandheldFriendly' content='true' />         <meta name='viewport' content='width=device-width, initial-scale=1.0' />          <link rel='preconnect' href='https://fonts.googleapis.com' />         <link rel='preconnect' href='https://fonts.gstatic.com' crossorigin />         <link href='https://fonts.googleapis.com/css2?family=Poppins:wght@200;300;400;500;600;700;800;900&display=swap' rel='stylesheet' />          <style type='text/css'>             /*********************  Default-CSS  *********************/              input[type='file']::-webkit-file-upload-button {                 cursor: pointer;             }              input[type='file']::-moz-file-upload-button {                 cursor: pointer;             }              input[type='file']::-ms-file-upload-button {                 cursor: pointer;             }              input[type='file']::-o-file-upload-button {                 cursor: pointer;             }              input[type='file'],             a[href],             input[type='submit'],             input[type='button'],             input[type='image'],             label[for],             select,             button,             .pointer {                 cursor: pointer;             }              ::-moz-focus-inner {                 border: 0px solid transparent;             }              ::-webkit-focus-inner {                 border: 0px solid transparent;             }              *::-moz-selection {                 color: #fff;                 background: #000;             }              *::-webkit-selection {                 color: #fff;                 background: #000;             }              *::-webkit-input-placeholder {                 color: #333333;                 opacity: 1;             }              *:-moz-placeholder {                 color: #333333;                 opacity: 1;             }              *::-moz-placeholder {                 color: #333333;                 opacity: 1;             }              *:-ms-input-placeholder {                 color: #333333;                 opacity: 1;             }             html {                 font-size: 18px;             }             html body {                 font-family: 'Poppins', sans-serif;                 margin: 0;                 line-height: 1.5;                 font-size: 1rem;             }              a,             div a:hover,             div a:active,             div a:focus,             button {                 text-decoration: none;                 -webkit-transition: all 0.5s ease 0s;                 -moz-transition: all 0.5s ease 0s;                 -ms-transition: all 0.5s ease 0s;                 -o-transition: all 0.5s ease 0s;                 transition: all 0.5s ease 0s;             }              a,             span,             div a:hover,             div a:active,             button {                 text-decoration: none;             }              *::after,             *::before,             * {                 -webkit-box-sizing: border-box;                 -moz-box-sizing: border-box;                 -ms-box-sizing: border-box;                 -o-box-sizing: border-box;                 box-sizing: border-box;             }              .no-list li,             .no-list ul,             .no-list ol,             footer li,             footer ul,             footer ol,             header li,             header ul,             header ol {                 list-style: inside none none;             }              .no-list ul,             .no-list ol,             footer ul,             footer ol,             header ul,             header ol {                 margin: 0;                 padding: 0;             }              a {                 outline: none;                 color: #555;             }              a:hover {                 color: #000;             }              body .clearfix,             body .clear {                 clear: both;                 line-height: 100%;             }              body .clearfix {                 height: auto;             }              * {                 outline: none !important;             }              table {                 border-collapse: collapse;                 border-spacing: 0;             }              ul:after,             li:after,             .clr:after,             .clearfix:after,             .container:after,             .grve-container:after {                 clear: both;                 display: block;                 content: '';             }              div input,             div select,             div textarea,             div button {                 font-family: 'Poppins', sans-serif;             }              body h1,             body h2,             body h3,             body h4,             body h5,             body h6 {                 font-family: 'Poppins', sans-serif;                 line-height: 1.5;                 color: #333;                 font-weight: 600;                 margin: 0 0 15px;             }              body h1:last-child,             body h2:last-child,             body h3:last-child,             body h4:last-child,             body h5:last-child,             body h6:last-child {                 margin-bottom: 0;             }              .h2,             h2 {                 font-size: 1.7rem;             }              div select {                 overflow: hidden;                 text-overflow: ellipsis;                 white-space: nowrap;             }              img,             svg {                 margin: 0 auto;                 max-width: 100%;                 max-height: 100%;                 width: auto;                 height: auto;                 vertical-align: top;             }              body p {                 margin: 0 0 25px;                 padding: 0;             }              body p:empty {                 margin: 0;                 line-height: 0;             }              body p:last-child {                 margin-bottom: 0;             }              p strong {                 font-weight: 600;             }              strong {                 font-weight: 600;             }              .a-left {                 text-align: left;             }              .a-right {                 text-align: right;             }              .a-center {                 text-align: center;             }              .hidden {                 display: none !important;             }              body .container .container {                 width: 100%;                 max-width: 100%;             }              /*********************  Default-CSS close  *********************/                    .back-btn {                 width: 48px;                 fill: #fff;margin-right: auto;} main{padding: 5% 10%;} .back-btn:hover, .action-btns:hover {                 opacity: 0.5;             }             .action-btns {                 background: transparent;                 border: none;                 height: 30px;fill: #fff;padding: 0 25px;} footer img {width: 100%;vertical-align: top; } </style></head>     <body>";

                    //middle body
                    var middleHtmlForDownload = "<main><table style='font-family: 'Poppins', sans-serif; font-weight: 400; width: 100%; border-collapse: collapse; border-spacing: 0; color: #333; line-height: 2;'><tbody><tr style='vertical-align: middle;'><td><img src= '" + url + "/assets/images/logo.png' alt='logo' /></td><td style='text-align: right;'><div style='margin-bottom: 15px;'> " +
                        "<span style='vertical-align: middle; display: inline-block; margin-right: 10px;'>+" + letterFormatDto.Phone1 + "<br />+" + letterFormatDto.Phone2 + "</span><span style='vertical-align: middle; display: inline-block;'><img src='" + url + "/assets/images/file-telephone.jpg' alt='file-telephone' /></span></div> <div style='margin-bottom: 15px;'><span style='vertical-align: middle; display: inline-block; margin-right: 10px;'>" + letterFormatDto.Phone1 + "<br />" + letterFormatDto.Email + "</span><span style='vertical-align: middle; display: inline-block;'><img src='" + url + "/assets/images/file-mail.jpg' alt='file-mail' /></span>  " +
                        "</div><div><span style='vertical-align: middle; display: inline-block; margin-right: 10px;'>Street Address Here <br />" + letterFormatDto.Address + "</span><span style='vertical-align: middle; display: inline-block;'><img src='" + url + "/assets/images/file-address.jpg' alt='file-address' /></span></div></td> </tr><tr><td height='100' colspan='2'></td></tr><tr style='vertical-align: bottom;'><td><div><strong style='color: #f17d2e; font-weight: 600; vertical-align: top;'>" + letterFormatDto.ReciverFullName + "</strong></div><div><small style='vertical-align: top;'>" + letterFormatDto.ReciverPossition + "</small></div><div><strong style='color: #f17d2e; vertical-align: top;'>A : </strong>" + letterFormatDto.ReciverAddress + "</div><div><strong style='vertical-align: top;'>W : </strong>" + letterFormatDto.ReciverEmail + "," + letterFormatDto.ReciverWebsite + "</div><div><strong style='color: #f17d2e; vertical-align: top;'>P:</strong>+" + letterFormatDto.ReciverPhone + "</div></td><td style='text-align: right;'> Date," + date.ToString("dd") + " " + date.ToString("MMMM") + ", " + date.Year + "</td></tr> <tr> <td height='100' colspan='2'></td></tr><tr><td colspan='2'>" + letterFormatDto.LetterFormatContent;


                    //footer html               
                    var footerHtmlForDownload = "</td></tr><tr><td height='100' colspan='2'></td></tr><tr><td colspan='2'> <div><img src='" + url + "/assets/images/signature.png' alt='signature' style='vertical-align: top;' /></div><div><strong style='vertical-align: top;'>" +
                        "John Smeeth</strong></div><div><small style='vertical-align: top;'>Manager</small></div>                         </td>                     </tr></tbody></table></main>   <footer style='height: 148px;'><img  src='" + url + "/assets/images/file-footer.png' alt='file-footer'/></footer></body> </html>";

               
                    var htmlContent = headerHtmlForDownload + middleHtmlForDownload + footerHtmlForDownload;
                    HtmlToPdf converter = new HtmlToPdf();
                    PdfDocument doc = converter.ConvertHtmlString(htmlContent);                 
                    var pdfString = Convert.ToBase64String(doc.Save()); 
                    return Json(new { message = pdfString });                   
                }                
                return null;
            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("string", ex.Message +"//"+ ex.InnerException);
                response.IsSuccess = false;
                response.Message = ex.Message;
                return null;
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddEditLetterFormat(LetterFormatDto input)
        {
            if (input.LetterFormatTitle != null)
            {
                if (input.Id > 0)
                {
                    var letterFormatData = await _letterFormatService.UpdateLetterFormat(input);
                    return Json(letterFormatData);
                }
                else
                {                   
                    var letterFormatData = await _letterFormatService.AddLetterFormat(input);
                    return Json(letterFormatData);
                }
            }

            return Json(new { message = "Please check the form again" });
        }
        [HttpPost]
        public async Task<JsonResult> DeleteLetterFormat(long id)
        {
            bool isLetterFormatDeleted = await _letterFormatService.DeleteLetterFormat(id);
            return Json(isLetterFormatDeleted);
        }
        #endregion
    }
}
