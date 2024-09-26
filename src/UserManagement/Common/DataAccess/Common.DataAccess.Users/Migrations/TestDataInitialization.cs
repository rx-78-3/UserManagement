using Dapper;
using Microsoft.Data.SqlClient;

namespace Common.DataAccess.Users.Migrations;

public static class TestDataInitialization
{
    public static async Task InitializeTestData(string connectionString)
    {
        using var connection = new SqlConnection(connectionString);

        var tableExists = await connection.ExecuteScalarAsync<int>(@"
                SELECT COUNT(*) 
                FROM Users");

        if (tableExists == 0)
        {
            // Password is always password123.
            await connection.ExecuteAsync(@"
                    INSERT INTO Users (Id, Username, PasswordHash, IsActive) VALUES 
                    ('9A8034C8-1DFB-4BD9-A16A-200979B8282F', 'someUser1',  '2d6e42cd548b8bbcd6064dbb2552a15ed289e9f33f28eac50ba6684f33e17165', 1),
                    ('9FDA9345-F78B-49A6-9ECA-1DBC45A8AC59', 'someUser2',  'c9d3278d777a4acb8d19d9d23327a717aa71f2578532723450278837c66997e3', 0),
                    ('3A124F6C-28BE-4FB6-8520-F5EADC640B25', 'someUser3',  '9fadba00451dbcbe590ae7457b884312852511de4fc4be338efad8f9ecea8f24', 1),
                    ('36DFCC9D-B45F-4C17-B071-5C0969A6C8EA', 'someUser4',  '6b02a9a8239943fba718552a7604a9f377288361c47a2fb8787c775ed533730a', 0),
                    ('C6ED6FF1-E24F-49F5-AB7D-4171AFCFBE70', 'someUser5',  'a690146f1b80ac2d593e91af931427e2761ab7fa902e74a4fc7cf90519a9e493', 1),
                    ('32DBB911-03C3-4652-BAA2-A62FFADD9DDD', 'someUser6',  '6fab4736afac1398f0ce54dc2c7512988f63b2376c91f0bbe37a797b606f4a52', 0),
                    ('FD2CC7AB-818E-4C8A-B616-31BF9ED16229', 'someUser7',  '3bdc9bdf610f5a2d048fff23134b8f1df1fb903d10e18156c82fcb8de43dce8e', 1),
                    ('65DD17B8-1E41-4647-A84C-6A31775CABA4', 'someUser8',  '05da6ca9caec83caec703fa8ddd1c518b37f7cd8abb0166f4eeba2d119c888c8', 0),
                    ('6E540F06-0EA0-4BF4-B7C8-6A6A2F2F9E3A', 'someUser9',  '6fd8e9dcdcd3a66e00f2bcccc3b445911516f4f775bb6876cb036357f7126859', 1),
                    ('E4D31163-BECA-4B53-99E2-7EEFD04C87D9', 'someUser10', 'f794231925865e3ef9c0e02b61db013bc4c46f866dda7ea1f505ed42237effbc', 0),
                    ('5CF7F927-49CD-4B83-8A18-2C296931C77E', 'someUser11', '25bb30da115f410477026e773531593b1275cbfd6bddec8ba3520695266192ec', 1),
                    ('9D303A84-D524-4914-A653-EBFFACC0ECED', 'someUser12', '70f748dc2919c25e0ae6bf681c9694e378e445c8c74fb49fde059fa34240a43c', 0),
                    ('DA6247F3-D8CE-4E4E-AE51-4377E7A1371B', 'someUser13', 'cd8abb5059981e0a0c45eb1e4b0c7938db25dda501462265bdd7459d545bd495', 1),
                    ('0AC47CBA-10D4-404C-8BF5-352C4452D0AB', 'someUser14', '7655775b0853b4bff1007ee42ac2f22a1d9daef46300b574dc0874b52ce78a9f', 0),
                    ('DFCE2EF7-0AB2-4B95-B3E6-57C9C33484BB', 'someUser15', '49b491f2e68cb358f460010987b94fcfc9f324ce50bd51a27728892344ded220', 1);");
        }
    }
}
