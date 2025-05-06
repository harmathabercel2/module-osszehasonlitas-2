/*
' Copyright (c) 2025 NiBeSzoCsa
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;
using System;
using System.Web.Caching;

namespace Dnn.BakeBeam.Osszehasonlitas.Models
{
    [TableName("ProductComparisonItem")]
    [PrimaryKey(nameof(Id), AutoIncrement = true)]
    public class ProductComparisonItem
    {
        public int Id { get; set; }                    // Egyedi azonosító
        public int ComparisonId { get; set; }          // Hivatkozás ProductComparison-ra
        public String ProductBvin { get; set; }          // Termék Bvin (GUID)
        public DateTime AddedUtc { get; set; }         // Kijelölés időpontja
        public int SortOrder { get; set; }             // Sorrend: 1 = első, 2 = második


    }
}
