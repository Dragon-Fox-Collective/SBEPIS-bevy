use proc_macro::TokenStream;
use quote::quote;
use syn::parse::Parse;
use syn::{parse_macro_input, Token, Path};

struct MacroInput<Input, Output> {
	a: Input,
	_comma1: Token![,],
	b: Input,
	_comma2: Token![,],
	tru: Output,
	_comma3: Token![,],
	fal: Output,
}

impl<Input: Parse, Output: Parse> Parse for MacroInput<Input, Output> {
	fn parse(input: syn::parse::ParseStream) -> syn::Result<Self> {
		Ok(Self {
			a: input.parse()?,
			_comma1: input.parse()?,
			b: input.parse()?,
			_comma2: input.parse()?,
			tru: input.parse()?,
			_comma3: input.parse()?,
			fal: input.parse()?,
		})
	}
}

#[proc_macro]
pub fn eq_tern(input: TokenStream) -> TokenStream
{
	let input = parse_macro_input!(input as MacroInput<Path, Path>);
	dbg!(&input.a);
	dbg!(&input.b);
	let out = if input.a.segments.last().unwrap().ident == input.b.segments.last().unwrap().ident {input.tru} else {input.fal};
	quote!{#out}.into()
}