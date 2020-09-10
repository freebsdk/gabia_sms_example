module GabiaSmsAgent

open System
open System.IO
open System.Net
open System.Text
open System.Web
open Newtonsoft.Json




module GabiaSMS =

    
    
    type OAuthResult = {
        access_token : string
        refresh_token : string
        expires_in : int32
        scope : string
        create_on : string
        is_expires : string
        token_type : string
        code : string
    }
    
    
        
    let DefaultRefKey() =
        "refkey_"+DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
        
        
        
        
        
    let BuildSMSPostData(phone : string) (callback : string) (message : string) (refkey : string) (is_foreign : string) =
        String.Format("phone={0}&callback={1}&message={2}&refkey={3}&is_foreign={4}",
                      phone, callback, message |> HttpUtility.UrlEncode, refkey |> HttpUtility.UrlEncode, is_foreign)
        
        
        
        
    let Base64AuthKey (id : string) (key_or_token: string) =
        (id + ":" + key_or_token)
        |> UTF8Encoding.ASCII.GetBytes
        |> Convert.ToBase64String
        
        
        
        
    let GabiaHttpRequest (uri : string) (post_data : string) (authroization : string)  =
        let request = HttpWebRequest.Create(uri) :?> HttpWebRequest
        request.Method <- "POST"
        request.ContentType <- "application/x-www-form-urlencoded"
        request.Headers.Add("Authorization", "Basic "+authroization);
        
        let byteData = post_data |> UTF8Encoding.UTF8.GetBytes
        request.ContentLength <- byteData.Length |> int64
        
        let postStream = request.GetRequestStream()
        postStream.Write(byteData, 0, byteData.Length)
        postStream.Close()
        
        request.KeepAlive <- false
        let response = request.GetResponse() :?> HttpWebResponse
        let resStream = response.GetResponseStream()
        
        let sr = new StreamReader(resStream)
        sr.ReadToEnd()
        
        
        
        
    let HostName =
        "https://sms.gabia.com/"

    let LoginUri =
        HostName + "/oauth/token"

    let SmsUri =
        HostName + "/api/send/sms"

    let LoginPostData = 
        "grant_type=client_credentials"
        

    
    
    let Login (id : string) (api_key : string) =
        GabiaHttpRequest
        <| LoginUri
        <| LoginPostData
        <| (Base64AuthKey <| id <| api_key) 
        |> JsonConvert.DeserializeObject<OAuthResult>
        
    
    
    
        
    let SendMessage (id : string) (auth_token : string) (phone : string) (callback : string) (message : string) (refkey : string) (is_foreign : string) =
        GabiaHttpRequest
        <| SmsUri
        <| (BuildSMSPostData <| phone <| callback <| message <| refkey <| is_foreign) 
        <| (Base64AuthKey <| id <| auth_token)            