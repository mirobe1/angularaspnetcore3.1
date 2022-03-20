export interface Account
{
         AccountId:number,
         FirstName:string,
         LastName:string,
         AccessToken:string,
         AccessTokenExpiryDate:Date,
         RefreshToken:string,
         RefreshTokenExpiryDate:Date,
         Role:string
}